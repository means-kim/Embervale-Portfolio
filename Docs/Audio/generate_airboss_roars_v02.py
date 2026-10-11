"""Version 2: breath-led, aperiodic frost-spirit roar auditions. No recordings or external voice models."""
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
    # Remove the harmonic buzz, fixed-rate tremolo and sub oscillator of v01.
    # Turbulent throat noise provides the body; a quiet broken vocal layer
    # adds creature identity without a continuously revving pitched drone.
    progress = np.clip((t-start)/(end-start), 0, 1)
    excitation = unit(band(rng.normal(size=n), 75, 3600))
    excitation *= np.clip(1 + .30*smooth_noise(rng, n, 21), .15, 1.8)
    voiced = np.zeros(n)
    for center, gain in formants:
        opening = np.clip(.55 + .35*np.sin(progress*np.pi) + .18*smooth_noise(rng,n,3.7),0,1)
        b1,a1 = signal.iirpeak(center*.83, 1.7, fs=SR)
        b2,a2 = signal.iirpeak(center*1.17, 1.7, fs=SR)
        low = unit(signal.lfilter(b1,a1,excitation))
        high = unit(signal.lfilter(b2,a2,excitation))
        voiced += gain*(low*(1-opening)+high*opening)
    throat = unit(voiced)
    broken_f0 = (pitch+38)*(1.3-.45*progress)
    broken_f0 *= np.clip(1+.21*smooth_noise(rng,n,29),.45,1.6)
    phase = np.cumsum(broken_f0)/SR*2*np.pi
    phase += 1.2*smooth_noise(rng,n,105)
    vocal = np.sin(phase) + .24*np.sin(2*phase+.7)
    vocal_gate = np.clip(smooth_noise(rng,n,4.1),0,1)
    grain = unit(band(rng.normal(size=n), 350, 2900))
    irregular = np.clip(.83+.17*smooth_noise(rng,n,8.3),.35,1.2)
    roar = (throat + rasp*.40*grain + .14*vocal*vocal_gate)*env*swell*irregular
    # Low-frequency pressure is non-periodic, not a motorcycle-like sub tone.
    pressure = unit(band(rng.normal(size=n), 48, 170))*env*.16
    air = unit(band(rng.normal(size=n), 600, 5200))
    air *= np.clip(.7+.23*smooth_noise(rng,n,9.1),.1,1.3)
    intake = np.sin(np.clip(t/start,0,1)*np.pi)**2 * (t<start)
    breath_env = .3*intake + env*(.22+.78*progress)
    dry = roar + pressure + wind*air*breath_env
    # Brief irregular shards, avoiding the continuous metallic tones in v01.
    shards = np.zeros(n)
    for onset in rng.uniform(start+.15,end-.1,11):
        u=t-onset
        pulse=np.exp(-np.maximum(u,0)*rng.uniform(22,45))*(u>=0)
        shards += pulse*unit(band(rng.normal(size=n),1400,4400))*.045
    dry += shards*env
    dry = signal.sosfilt(signal.butter(2, 38, fs=SR, btype='highpass', output='sos'), dry)
    channels=[]
    # Diffuse short reverb rather than a repeated delay/pulse pattern.
    for side in range(2):
        size=int(.72*SR)
        rt=np.arange(size)/SR
        impulse=rng.normal(size=size)*np.exp(-rt*9)
        impulse[:int(.027*SR)]=0
        impulse=signal.sosfilt(signal.butter(2,2300,fs=SR,output='sos'),impulse)
        impulse/=max(np.linalg.norm(impulse),1e-8)
        wet=signal.fftconvolve(dry,impulse)[:n]
        wet*=np.sqrt(np.mean(dry*dry))/max(np.sqrt(np.mean(wet*wet)),1e-8)
        channels.append(dry+.16*width*wet)
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
        ('AirBoss_Roar_01_FrostLord_v02',101,4.2,64,[(270,1),(720,.7),(1280,.3)],.28,.19,.5,.08),
        ('AirBoss_Roar_02_BlizzardWraith_v02',202,4.4,94,[(430,.9),(1080,.7),(1900,.35)],.43,.48,1,.16),
        ('AirBoss_Roar_03_FrozenAbyss_v02',303,4.6,43,[(190,1),(540,.65),(960,.3)],.38,.16,.7,.12),
        ('AirBoss_Roar_04_ShatteredIce_v02',404,3.8,78,[(340,.9),(870,.8),(1670,.45)],.68,.27,.65,.3),
        ('AirBoss_Roar_05_FinalFury_v02',505,4.5,56,[(250,1),(650,.8),(1430,.5)],.55,.35,1,.22),
    ]
    report=[render(*v) for v in variants]
    (ROOT/'Docs/Audio/AirBoss_Roar_v02_manifest.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
    print(json.dumps(report,indent=2))
