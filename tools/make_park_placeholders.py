#!/usr/bin/env python3
"""Write simple labeled PNG placeholders for the L1 park-gate scene."""
import os
import struct
import zlib
import uuid

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
ART = os.path.join(ROOT, "Assets", "Art")
RES = os.path.join(ROOT, "Assets", "Resources", "Art")

# name -> (w, h, rgb, kind)
SPECS = {
    "bg_park_gate": (512, 640, (168, 216, 240), "bg"),
    "char_bunny": (256, 256, (247, 186, 209), "circle"),
    "char_fox": (256, 256, (249, 145, 61), "circle"),
    "prop_tree": (256, 256, (76, 175, 80), "tree"),
    "prop_rock": (256, 256, (158, 153, 143), "rock"),
    "prop_ball": (256, 256, (237, 84, 79), "circle"),
    "drop_fox": (256, 256, (249, 145, 61), "ring"),
}


def png(path, w, h, pixels):
    def chunk(tag, data):
        crc = zlib.crc32(tag + data) & 0xFFFFFFFF
        return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", crc)

    raw = bytearray()
    for y in range(h):
        raw.append(0)
        raw.extend(pixels[y * w * 4 : (y + 1) * w * 4])
    ihdr = struct.pack(">IIBBBBB", w, h, 8, 6, 0, 0, 0)
    body = b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", ihdr) + chunk(b"IDAT", zlib.compress(bytes(raw), 9)) + chunk(b"IEND", b"")
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "wb") as f:
        f.write(body)


def fill(w, h, rgba):
    buf = bytearray(w * h * 4)
    r, g, b, a = rgba
    for i in range(0, len(buf), 4):
        buf[i : i + 4] = bytes((r, g, b, a))
    return buf


def disk(buf, w, h, cx, cy, rad, rgba):
    r2 = rad * rad
    for y in range(h):
        for x in range(w):
            dx, dy = x + 0.5 - cx, y + 0.5 - cy
            if dx * dx + dy * dy <= r2:
                i = (y * w + x) * 4
                buf[i : i + 4] = bytes(rgba)


def rect(buf, w, h, x0, y0, x1, y1, rgba):
    for y in range(max(0, y0), min(h, y1)):
        for x in range(max(0, x0), min(w, x1)):
            i = (y * w + x) * 4
            buf[i : i + 4] = bytes(rgba)


def make(name, w, h, rgb, kind):
    r, g, b = rgb
    if kind == "bg":
        buf = fill(w, h, (r, g, b, 255))
        rect(buf, w, h, 0, int(h * 0.62), w, h, (120, 186, 110, 255))  # grass
        rect(buf, w, h, int(w * 0.18), int(h * 0.28), int(w * 0.82), int(h * 0.62), (186, 140, 90, 255))  # gate
        rect(buf, w, h, int(w * 0.42), int(h * 0.32), int(w * 0.58), int(h * 0.62), (168, 216, 240, 255))  # opening
        return buf
    buf = fill(w, h, (0, 0, 0, 0))
    cx, cy = w * 0.5, h * 0.5
    if kind == "circle":
        disk(buf, w, h, cx, cy, w * 0.42, (r, g, b, 255))
        disk(buf, w, h, cx, cy, w * 0.16, (255, 255, 255, 220))
    elif kind == "tree":
        disk(buf, w, h, cx, h * 0.42, w * 0.34, (r, g, b, 255))
        rect(buf, w, h, int(w * 0.44), int(h * 0.52), int(w * 0.56), int(h * 0.92), (121, 85, 72, 255))
    elif kind == "rock":
        disk(buf, w, h, cx, cy + 8, w * 0.36, (r, g, b, 255))
        disk(buf, w, h, cx - 20, cy + 16, w * 0.22, (r - 20, g - 20, b - 18, 255))
    elif kind == "ring":
        disk(buf, w, h, cx, cy, w * 0.44, (r, g, b, 90))
        disk(buf, w, h, cx, cy, w * 0.30, (0, 0, 0, 0))
    return buf


META = """fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 12
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 1
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 512
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 100
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 0
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 512
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: {sprite}
    internalID: 1537655665
    vertices: []
    indices: 
    edges: []
    weights: []
  spritePackingTag: 
  pSDRemoveMatte: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def write_meta(path):
    guid = uuid.uuid4().hex
    sprite = uuid.uuid4().hex
    with open(path + ".meta", "w") as f:
        f.write(META.format(guid=guid, sprite=sprite))


def main():
    os.makedirs(ART, exist_ok=True)
    os.makedirs(RES, exist_ok=True)
    for name, (w, h, rgb, kind) in SPECS.items():
        buf = make(name, w, h, rgb, kind)
        for folder in (ART, RES):
            dest = os.path.join(folder, name + ".png")
            png(dest, w, h, buf)
            write_meta(dest)
            print("wrote", dest)


if __name__ == "__main__":
    main()
