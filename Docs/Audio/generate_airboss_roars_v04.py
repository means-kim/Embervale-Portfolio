"""Version 4: dry creature throat roars; no breath bed, reduced turbulence and reverb. No recordings or external voice models."""
from pathlib import Path
import json
import uuid
import wave
import numpy as np
from scipy import signal

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / 'Assets/06.Sounds/Monster/AirBoss'
SR = 48000


def band(x, low, high):
    return signal.sosfilt(signal.butter(3, [low, high], fs=SR, btype='bandpass', output='sos'), x)


def unit(x):
    return x / max(np.sqrt(np.mean(x*x)), 1e-8)


def smooth_noise(rng, n, rate):
    return np.interp(np.arange(n), np.linspace(0, n-1, max(3, int(n/SR*rate))), rng.normal(size=max(3, int(n/SR*rate))))


def render(name, seed, duration, pitch, formants, rasp, wind, width, pulses):
    rng = np.random.default_rng(seed)
    t = np.arange(int(duration*SR)) / SR
    n = len(t)
    # A breath intake, open-throat attack, sustained growl, and exhaled release.
    start, end = .23, duration - .85
    attack = np.clip((t-start)/.18, 0, 1)
    release = np.clip((end-t)/.65, 0, 1)
    env = np.sin(attack*np.pi/2)**2 * np.sin(release*np.pi/2)**2
    swell = .78 + .22*np.sin(np.clip((t-start)/(end-start), 0, 1)*np.pi)
    progress = np.clip((t-start)/(end-start), 0, 1)
    # A rising open-mouthed bellow collapsing into a low guttural growl.
    contour = np.interp(progress, [0,.13,.36,.65,1], [.77,1.32,1.10,.78,.56])
    excitation = np.zeros(n)
    # Individual asymmetric vocal-fold pulses with non-repeating cycle jitter.
    # No stable oscillator bass or engine-like periodic tremolo.
    cursor = 0
    while cursor < n:
        local_pitch = pitch * contour[cursor] * np.exp(rng.normal(0,.16))
        count = max(70, int(SR/max(30,local_pitch)))
        count = min(count,n-cursor)
        u = np.linspace(0,1,count,endpoint=False)
        opening = rng.uniform(.31,.56)
        pulse = np.where(u<opening, np.sin(np.pi*np.minimum(u/opening,1))**2,
                         -.38*np.exp(-np.maximum(u-opening,0)*rng.uniform(10,19)))
        pulse -= pulse.mean()
        excitation[cursor:cursor+count] = pulse*rng.uniform(.55,1.3)
        cursor += count
    # A second ragged throat layer, much quieter than the primary vocal body.
    rough = unit(band(rng.normal(size=n),110,850))
    rough *= np.clip(.4+.4*smooth_noise(rng,n,36),0,1.3)
    excitation = unit(excitation) + rough*.035
    voiced=np.zeros(n)
    opening = np.interp(progress,[0,.17,.55,1],[.10,1,.55,0])
    for center,gain in formants:
        b1,a1=signal.iirpeak(center*.74,2.8,fs=SR)
        b2,a2=signal.iirpeak(center*1.2,2.4,fs=SR)
        low=unit(signal.lfilter(b1,a1,excitation))
        high=unit(signal.lfilter(b2,a2,excitation))
        voiced += gain*((1-opening)*low+opening*high)
    body=unit(band(excitation,55,260))
    throat=unit(voiced)
    # A few uneven surges, not repeated pulsing; lower register remains central.
    irregular=np.clip(.9+.12*smooth_noise(rng,n,4.3),.55,1.15)
    roar=(throat*.92+body*.45+rough*rasp*.025)*env*swell*irregular
    # Remove the separate air/intake and rumbling noise beds entirely.
    dry=roar
    dry=signal.sosfilt(signal.butter(2,2400,fs=SR,output='sos'),dry)
    dry = signal.sosfilt(signal.butter(2, 38, fs=SR, btype='highpass', output='sos'), dry)
    channels=[]
    # Diffuse short reverb rather than a repeated delay/pulse pattern.
    for side in range(2):
        size=int(.35*SR)
        rt=np.arange(size)/SR
        impulse=rng.normal(size=size)*np.exp(-rt*17)
        impulse[:int(.027*SR)]=0
        impulse=signal.sosfilt(signal.butter(2,2300,fs=SR,output='sos'),impulse)
        impulse/=max(np.linalg.norm(impulse),1e-8)
        wet=signal.fftconvolve(dry,impulse)[:n]
        wet*=np.sqrt(np.mean(dry*dry))/max(np.sqrt(np.mean(wet*wet)),1e-8)
        channels.append(dry+.025*width*wet)
    audio=np.stack(channels,axis=1)
    # Soft transient control; conservative common RMS target and -3 dBFS ceiling.
    audio=np.tanh(audio*.30)
    active=audio[int(start*SR):int(end*SR)]
    audio*=10**(-19/20)/max(np.sqrt(np.mean(active*active)),1e-9)
    peak=np.max(np.abs(audio))
    if peak>10**(-3/20): audio*=10**(-3/20)/peak
    fade=int(.04*SR)
    audio[:fade]*=np.linspace(0,1,fade)[:,None]
    audio[-fade:]*=np.linspace(1,0,fade)[:,None]
    path=OUT/(name+'.wav')
    with wave.open(str(path),'wb') as f:
        f.setnchannels(2); f.setsampwidth(2); f.setframerate(SR)
        f.writeframes((audio*32767).astype('<i2').tobytes())
    meta=Path(str(path)+'.meta')
    if not meta.exists():
        template=(OUT/'AirBoss_Blizzard_RisingGust_1s_v01.wav.meta').read_text(encoding='utf-8-sig')
        import re
        meta.write_text(re.sub(r'(?m)^guid: .*$', 'guid: '+uuid.uuid4().hex,template),encoding='utf-8')
    return dict(file=path.name,seconds=duration,peak_dbfs=round(20*np.log10(np.max(np.abs(audio))),2),method='procedural synthesis',seed=seed)


if __name__=='__main__':
    variants=[
        ('AirBoss_Roar_01_FrostLord_v04',101,4.2,64,[(270,1),(720,.7),(1280,.3)],.28,.19,.5,.08),
        ('AirBoss_Roar_02_BlizzardWraith_v04',202,4.4,76,[(320,1),(810,.6),(1450,.25)],.43,.48,1,.16),
        ('AirBoss_Roar_03_FrozenAbyss_v04',303,4.6,48,[(180,1),(440,.6),(880,.25)],.38,.16,.7,.12),
        ('AirBoss_Roar_04_ShatteredIce_v04',404,3.8,72,[(310,1),(760,.7),(1350,.3)],.68,.27,.65,.3),
        ('AirBoss_Roar_05_FinalFury_v04',505,4.5,56,[(250,1),(650,.8),(1430,.5)],.55,.35,1,.22),
    ]
    report=[render(*v) for v in variants]
    (ROOT/'Docs/Audio/AirBoss_Roar_v04_manifest.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
    print(json.dumps(report,indent=2))
