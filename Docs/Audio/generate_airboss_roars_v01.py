"""Procedural frost-spirit roar auditions. No recordings or external voice models."""
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
    f0 = pitch * (1.28 - .40*np.clip((t-start)/(end-start), 0, 1))
    f0 *= 1 + .025*smooth_noise(rng, n, 18) + .018*np.sin(2*np.pi*6.3*t)
    phase = np.cumsum(f0)/SR*2*np.pi
    # Irregular vocal-fold excitation, subharmonics and breath-shaped formants.
    source = np.zeros(n)
    for k in range(1, 28):
        source += np.sin(k*phase + .12*np.sin(phase*.47)) / k**.9
    source = np.tanh(source*1.6)
    source *= 1 + .23*np.sin(phase*.5) + .13*smooth_noise(rng, n, 38)
    voiced = np.zeros(n)
    for center, gain in formants:
        b, a = signal.iirpeak(center, 2.7, fs=SR)
        voiced += gain*unit(signal.lfilter(b, a, source))
    grain = unit(band(rng.normal(size=n), 130, 2600))
    voiced = unit(voiced) + rasp*grain*(.5+.5*np.abs(np.sin(phase*.49)))
    roar = unit(np.tanh(voiced*.85)) * env * swell
    roar *= 1 - pulses*(.5+.5*np.sin(2*np.pi*3.7*t))
    sub = np.sin(phase*.5 + .05*np.sin(2*np.pi*9*t)) * env * .20
    air = unit(band(rng.normal(size=n), 400, 4200))
    air *= .65+.25*np.tanh(smooth_noise(rng,n,12))
    intake = np.sin(np.clip(t/start,0,1)*np.pi)**2 * (t<start)
    breath_env = .35*intake + env*(.35+.65*np.clip((t-start)/(end-start),0,1))
    dry = roar + sub + wind*air*breath_env
    # Quiet, inharmonic icy resonances: texture rather than a musical chord.
    frost = np.zeros(n)
    for freq in [1271, 1837, 2611, 3307]:
        frost += np.sin(2*np.pi*freq*t + smooth_noise(rng,n,4))
    dry += .024*frost*env*(.5+.5*np.sin(2*np.pi*1.8*t))
    dry = signal.sosfilt(signal.butter(2, 38, fs=SR, btype='highpass', output='sos'), dry)
    channels=[]
    for side in range(2):
        y=dry.copy()
        for delay, gain in [(.041,.15),(.089,.13),(.157,.11),(.263,.08),(.419,.06),(.613,.035)]:
            lag=int((delay+side*width*.009)*SR)
            tail=signal.sosfilt(signal.butter(1,2600,fs=SR,output='sos'),dry)
            y[lag:]+=gain*tail[:-lag]
        channels.append(y)
    audio=np.stack(channels,axis=1)
    # Soft transient control; conservative common RMS target and -3 dBFS ceiling.
    audio=np.tanh(audio*.7)
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
        ('AirBoss_Roar_01_FrostLord_v01',101,4.2,64,[(270,1),(720,.7),(1280,.3)],.28,.19,.5,.08),
        ('AirBoss_Roar_02_BlizzardWraith_v01',202,4.4,94,[(430,.9),(1080,.7),(1900,.35)],.43,.48,1,.16),
        ('AirBoss_Roar_03_FrozenAbyss_v01',303,4.6,43,[(190,1),(540,.65),(960,.3)],.38,.16,.7,.12),
        ('AirBoss_Roar_04_ShatteredIce_v01',404,3.8,78,[(340,.9),(870,.8),(1670,.45)],.68,.27,.65,.3),
        ('AirBoss_Roar_05_FinalFury_v01',505,4.5,56,[(250,1),(650,.8),(1430,.5)],.55,.35,1,.22),
    ]
    report=[render(*v) for v in variants]
    (ROOT/'Docs/Audio/AirBoss_Roar_v01_manifest.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
    print(json.dumps(report,indent=2))
