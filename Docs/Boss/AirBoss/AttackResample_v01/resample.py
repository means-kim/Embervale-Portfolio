from pathlib import Path
from PIL import Image
import json

root = Path(__file__).resolve().parent
report = []
for path in (root / 'Paths.txt').read_text(encoding='utf-8-sig').splitlines():
    name = Path(path).name
    with Image.open(root / 'Original' / name) as image:
        # Premultiplied-alpha Lanczos prevents dark fringes around transparency.
        factor = 4 if max(image.size) <= 2048 else 2
        target = (image.width * factor, image.height * factor)
        out = image.convert('RGBA').convert('RGBa').resize(target, Image.Resampling.LANCZOS).convert('RGBA')
        out.save(root / 'Prepared' / name, compress_level=6)
        report.append(dict(path=path, before=image.size, after=target))
        print(name, target, flush=True)
        out.close()
(root / 'Dimensions.json').write_text(json.dumps(report, indent=2), encoding='utf-8')

