#!/usr/bin/env python3
import math
import random
import struct
import zlib
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
ROOM = ROOT / "Game" / "Rooms" / "Forest" / "Sprites"
CHAR = ROOT / "Game" / "Characters" / "Dave" / "Sprites"


def write_png(path, width, height, pixels):
    rows = []
    for y in range(height):
        start = y * width * 4
        rows.append(b"\x00" + bytes(pixels[start:start + width * 4]))
    raw = b"".join(rows)

    def chunk(kind, data):
        body = kind + data
        return struct.pack(">I", len(data)) + body + struct.pack(">I", zlib.crc32(body) & 0xFFFFFFFF)

    data = b"\x89PNG\r\n\x1a\n"
    data += chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0))
    data += chunk(b"IDAT", zlib.compress(raw, 9))
    data += chunk(b"IEND", b"")
    path.write_bytes(data)


def canvas(width, height, color=(0, 0, 0, 0)):
    r, g, b, a = color
    return bytearray([r, g, b, a] * width * height)


def blend(px, w, h, x, y, color):
    if x < 0 or y < 0 or x >= w or y >= h:
        return
    r, g, b, a = color
    i = (y * w + x) * 4
    inv = 255 - a
    px[i] = (r * a + px[i] * inv) // 255
    px[i + 1] = (g * a + px[i + 1] * inv) // 255
    px[i + 2] = (b * a + px[i + 2] * inv) // 255
    px[i + 3] = min(255, a + px[i + 3] * inv // 255)


def rect(px, w, h, x0, y0, x1, y1, color):
    x0, y0, x1, y1 = map(int, (x0, y0, x1, y1))
    for y in range(max(0, y0), min(h, y1)):
        for x in range(max(0, x0), min(w, x1)):
            blend(px, w, h, x, y, color)


def line(px, w, h, x0, y0, x1, y1, color, thickness=1):
    x0, y0, x1, y1 = map(int, (x0, y0, x1, y1))
    dx, dy = abs(x1 - x0), -abs(y1 - y0)
    sx = 1 if x0 < x1 else -1
    sy = 1 if y0 < y1 else -1
    err = dx + dy
    while True:
        for oy in range(-thickness, thickness + 1):
            for ox in range(-thickness, thickness + 1):
                if ox * ox + oy * oy <= thickness * thickness:
                    blend(px, w, h, x0 + ox, y0 + oy, color)
        if x0 == x1 and y0 == y1:
            break
        e2 = 2 * err
        if e2 >= dy:
            err += dy
            x0 += sx
        if e2 <= dx:
            err += dx
            y0 += sy


def ellipse(px, w, h, cx, cy, rx, ry, color):
    for y in range(int(cy - ry), int(cy + ry) + 1):
        for x in range(int(cx - rx), int(cx + rx) + 1):
            if rx and ry and ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2 <= 1:
                blend(px, w, h, x, y, color)


def poly(px, w, h, pts, color):
    ys = [p[1] for p in pts]
    for y in range(max(0, int(min(ys))), min(h, int(max(ys)) + 1)):
        xs = []
        for i, (x1, y1) in enumerate(pts):
            x2, y2 = pts[(i + 1) % len(pts)]
            if (y1 <= y < y2) or (y2 <= y < y1):
                xs.append(x1 + (y - y1) * (x2 - x1) / (y2 - y1))
        xs.sort()
        for a, b in zip(xs[0::2], xs[1::2]):
            rect(px, w, h, a, y, b + 1, y + 1, color)


def draw_background():
    w, h = 2532, 1117
    px = canvas(w, h, (5, 8, 13, 255))
    random.seed(19)

    for y in range(h):
        t = y / h
        base = int(8 + 22 * t)
        rect(px, w, h, 0, y, w, y + 1, (base // 2, base, base + 6, 255))

    # Alley walls and wet floor.
    poly(px, w, h, [(0, 140), (760, 260), (1030, 1117), (0, 1117)], (10, 14, 18, 255))
    poly(px, w, h, [(2532, 120), (1780, 260), (1490, 1117), (2532, 1117)], (8, 11, 15, 255))
    poly(px, w, h, [(620, 625), (1900, 620), (2532, 1117), (0, 1117)], (18, 21, 22, 255))
    poly(px, w, h, [(935, 585), (1588, 582), (2055, 1117), (485, 1117)], (24, 25, 24, 255))

    # Bricks.
    for side, x0, x1, tilt in [("l", 0, 900, 0.2), ("r", 1660, 2532, -0.16)]:
        for y in range(185, 675, 58):
            line(px, w, h, x0, y, x1, int(y + (x1 - x0) * tilt), (34, 42, 46, 95), 2)
        for x in range(x0 + 35, x1, 115):
            line(px, w, h, x, 170, int(x + (675 - 170) * tilt), 675, (24, 32, 35, 75), 1)

    # Service door.
    rect(px, w, h, 1856, 320, 2170, 780, (10, 14, 16, 255))
    rect(px, w, h, 1880, 346, 2147, 774, (25, 31, 33, 255))
    rect(px, w, h, 1908, 380, 2118, 742, (15, 20, 23, 255))
    line(px, w, h, 2016, 380, 2016, 742, (42, 47, 47, 140), 2)
    rect(px, w, h, 2108, 560, 2126, 578, (185, 155, 86, 230))
    rect(px, w, h, 1826, 286, 2198, 322, (6, 8, 10, 255))

    # Lamp and light cone.
    rect(px, w, h, 690, 240, 725, 610, (9, 10, 11, 255))
    rect(px, w, h, 628, 220, 788, 258, (14, 14, 13, 255))
    ellipse(px, w, h, 710, 290, 90, 54, (214, 174, 83, 58))
    ellipse(px, w, h, 710, 295, 36, 24, (241, 198, 96, 175))
    for r in range(14):
        ellipse(px, w, h, 850, 690 + r * 7, 360 + r * 26, 96 + r * 10, (180, 140, 72, max(2, 24 - r)))

    # Puddles and perspective cracks.
    for i in range(35):
        x = random.randint(430, 2160)
        y = random.randint(700, 1080)
        rx = random.randint(35, 190)
        ry = random.randint(4, 14)
        ellipse(px, w, h, x, y, rx, ry, (75, 91, 96, random.randint(20, 55)))
    for x in range(580, 2060, 190):
        line(px, w, h, x, 1120, 1240 + (x - 1300) // 7, 610, (5, 6, 7, 110), 2)
    for y in range(705, 1110, 85):
        line(px, w, h, 500, y, 2030, y - 8, (43, 45, 42, 80), 1)

    # Body outline / coat and evidence location.
    poly(px, w, h, [(1010, 805), (1210, 760), (1370, 850), (1270, 940), (1050, 918)], (6, 7, 8, 225))
    ellipse(px, w, h, 1160, 770, 62, 25, (13, 12, 11, 215))
    line(px, w, h, 1122, 840, 1305, 895, (42, 38, 32, 100), 4)
    ellipse(px, w, h, 1338, 905, 38, 12, (83, 10, 13, 170))

    # Rain.
    for _ in range(850):
        x = random.randint(-50, w + 40)
        y = random.randint(0, h)
        length = random.randint(22, 70)
        alpha = random.randint(28, 94)
        line(px, w, h, x, y, x - 18, y + length, (140, 162, 171, alpha), 1)

    # Vignette.
    cx, cy = w / 2, h * 0.58
    maxd = math.hypot(cx, cy)
    for y in range(0, h, 2):
        for x in range(0, w, 2):
            d = math.hypot(x - cx, y - cy) / maxd
            a = int(max(0, (d - 0.45) * 210))
            if a:
                rect(px, w, h, x, y, x + 2, y + 2, (0, 0, 0, min(185, a)))

    write_png(ROOM / "Back_0.png", w, h, px)


def draw_room_overlays():
    # Drain overlay in the old "Well" slot.
    w, h = 2532, 1117
    px = canvas(w, h)
    ellipse(px, w, h, 1590, 850, 155, 42, (5, 7, 8, 230))
    ellipse(px, w, h, 1590, 846, 132, 28, (26, 30, 29, 230))
    for dx in range(-105, 126, 35):
        rect(px, w, h, 1590 + dx, 817, 1590 + dx + 12, 875, (4, 5, 5, 220))
    line(px, w, h, 1440, 884, 1302, 934, (116, 11, 15, 190), 7)
    line(px, w, h, 1320, 933, 1238, 963, (155, 16, 20, 135), 4)
    for i in range(16):
        ellipse(px, w, h, 1430 - i * 22, 886 + i * 7, 15 + i, 5, (95, 7, 12, 80))
    write_png(ROOM / "Well_0.png", w, h, px)

    # Bottom foreground wet curb.
    w, h = 2549, 114
    px = canvas(w, h)
    rect(px, w, h, 0, 42, w, h, (2, 4, 6, 210))
    for x in range(0, w, 120):
        line(px, w, h, x, 60, x + 90, 52, (55, 68, 72, 70), 1)
    for x in range(80, w, 260):
        ellipse(px, w, h, x, 82, 90, 7, (105, 120, 122, 45))
    write_png(ROOM / "Foreground_0.png", w, h, px)

    # Left and right foreground wall silhouettes.
    for name, w, h, side in [("ForegroundL_0.png", 653, 1144, "left"), ("ForegroundR_0.png", 454, 1128, "right")]:
        px = canvas(w, h)
        if side == "left":
            poly(px, w, h, [(0, 0), (520, 0), (270, h), (0, h)], (0, 1, 3, 170))
            for y in range(130, h, 95):
                line(px, w, h, 0, y, 430, y + 36, (42, 50, 54, 58), 2)
        else:
            poly(px, w, h, [(95, 0), (w, 0), (w, h), (275, h)], (0, 1, 3, 174))
            for y in range(125, h, 95):
                line(px, w, h, 90, y + 28, w, y, (42, 50, 54, 58), 2)
        write_png(ROOM / name, w, h, px)

    # Severed hand prop in the old bucket slot.
    w, h = 82, 80
    px = canvas(w, h)
    poly(px, w, h, [(10, 58), (30, 48), (55, 52), (74, 63), (46, 76), (18, 70)], (206, 200, 177, 225))
    ellipse(px, w, h, 38, 50, 18, 12, (159, 122, 93, 255))
    rect(px, w, h, 25, 54, 51, 68, (151, 112, 88, 255))
    for i, x in enumerate([20, 29, 38, 47]):
        line(px, w, h, x, 45, x - 8 + i * 2, 25 + i * 2, (165, 128, 100, 255), 4)
        ellipse(px, w, h, x - 9 + i * 2, 23 + i * 2, 4, 5, (174, 136, 106, 255))
    line(px, w, h, 51, 50, 68, 37, (162, 123, 95, 255), 5)
    ellipse(px, w, h, 69, 36, 5, 5, (174, 136, 106, 255))
    rect(px, w, h, 17, 61, 48, 67, (105, 6, 9, 210))
    ellipse(px, w, h, 51, 64, 10, 5, (146, 9, 13, 180))
    write_png(ROOM / "Bucket_0.png", w, h, px)


def draw_detective_sprite(path, facing="right", frame=0, talk=False, walk=False):
    w, h = 224, 320
    px = canvas(w, h)
    cx = 112
    sway = (frame % 4 - 1.5) * 3 if walk else 0
    if facing in ("right", "ur"):
        head_x = cx + 12
        nose = 1
    elif facing == "left":
        head_x = cx - 12
        nose = -1
    else:
        head_x = cx
        nose = 0

    # Coat shadow.
    ellipse(px, w, h, cx, 292, 45, 10, (0, 0, 0, 80))
    poly(px, w, h, [(74, 126), (150, 126), (172, 278), (52, 278)], (14, 17, 19, 255))
    poly(px, w, h, [(92, 132), (132, 132), (143, 270), (80, 270)], (32, 36, 37, 255))
    poly(px, w, h, [(72, 128), (102, 132), (82, 246), (56, 255)], (8, 10, 12, 255))
    poly(px, w, h, [(151, 128), (124, 132), (145, 246), (172, 255)], (8, 10, 12, 255))
    rect(px, w, h, 101, 142, 123, 271, (9, 11, 12, 230))

    # Legs.
    leg_shift = int(sway)
    rect(px, w, h, 83 + leg_shift, 262, 103 + leg_shift, 304, (8, 9, 11, 255))
    rect(px, w, h, 122 - leg_shift, 262, 143 - leg_shift, 304, (8, 9, 11, 255))
    ellipse(px, w, h, 93 + leg_shift, 306, 18, 6, (4, 5, 6, 255))
    ellipse(px, w, h, 134 - leg_shift, 306, 18, 6, (4, 5, 6, 255))

    # Collar, head, hair.
    poly(px, w, h, [(78, 125), (112, 98), (150, 126), (136, 154), (112, 136), (88, 154)], (58, 54, 48, 255))
    ellipse(px, w, h, head_x, 78, 26, 32, (146, 111, 88, 255))
    ellipse(px, w, h, head_x - 8, 75, 33, 39, (14, 12, 13, 255))
    rect(px, w, h, head_x - 26, 84, head_x + 10, 128, (13, 11, 12, 255))
    rect(px, w, h, head_x - 18, 104, head_x + 24, 146, (12, 10, 11, 225))
    ellipse(px, w, h, head_x + nose * 16, 80, 3, 3, (222, 183, 130, 230))
    if talk:
        rect(px, w, h, head_x + nose * 8 - 2, 96, head_x + nose * 8 + 8, 100, (42, 13, 14, 210))
    else:
        line(px, w, h, head_x + nose * 3, 96, head_x + nose * 12, 96, (42, 20, 18, 190), 1)

    # Hat/coat highlight and arms.
    rect(px, w, h, head_x - 36, 49, head_x + 34, 58, (15, 16, 16, 255))
    rect(px, w, h, head_x - 22, 37, head_x + 20, 52, (13, 14, 15, 255))
    line(px, w, h, 72, 150, 54, 230, (11, 12, 13, 255), 8)
    line(px, w, h, 151, 150, 170, 228, (11, 12, 13, 255), 8)
    line(px, w, h, 92, 137, 83, 250, (73, 68, 56, 125), 2)
    line(px, w, h, 136, 137, 145, 250, (73, 68, 56, 125), 2)

    # Rain glints.
    for y in range(136, 260, 24):
        line(px, w, h, 88, y, 72, y + 35, (104, 126, 134, 55), 1)
        line(px, w, h, 148, y, 132, y + 35, (104, 126, 134, 45), 1)

    write_png(path, w, h, px)


def draw_characters():
    groups = {
        "IdleD_0.png": ("down", 0, False, False),
        "IdleR_0.png": ("right", 0, False, False),
        "IdleUR_0.png": ("ur", 0, False, False),
        "IdleU_0.png": ("up", 0, False, False),
        "TalkD_0.png": ("down", 0, True, False),
        "TalkD_1.png": ("down", 1, True, False),
        "TalkR_0.png": ("right", 0, True, False),
        "TalkR_1.png": ("right", 1, True, False),
        "TalkR_2.png": ("right", 2, True, False),
        "TalkUR_0.png": ("ur", 0, True, False),
        "TalkUR_1.png": ("ur", 1, True, False),
        "TalkU_0.png": ("up", 0, True, False),
        "TalkU_1.png": ("up", 1, True, False),
    }
    for name, spec in groups.items():
        draw_detective_sprite(CHAR / name, *spec)
    for i in range(4):
        draw_detective_sprite(CHAR / f"WalkR_{i}.png", "right", i, False, True)
    for i in range(4):
        draw_detective_sprite(CHAR / f"WalkUR_{i}.png", "ur", i, False, True)


if __name__ == "__main__":
    draw_background()
    draw_room_overlays()
    draw_characters()
