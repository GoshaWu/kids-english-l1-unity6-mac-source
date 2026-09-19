#!/usr/bin/env python3
"""P0 L1 placeholder anim chips + voice/sfx tones. Exact Art ids."""
import math
import os
import struct
import uuid
import wave
import zlib

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
ART = os.path.join(ROOT, "Assets", "Art")
RES_ART = os.path.join(ROOT, "Assets", "Resources", "Art")
AUDIO = os.path.join(ROOT, "Assets", "Audio")
RES_AUDIO = os.path.join(ROOT, "Assets", "Resources", "Audio")
SFX = os.path.join(ROOT, "Assets", "Audio", "Sfx")
RES_SFX = os.path.join(ROOT, "Assets", "Resources", "Audio")

ANIM = {
    "anim_bunny_wave": (247, 186, 209),
    "anim_wave_bunny": (247, 186, 209),
    "anim_fox_wave": (249, 145, 61),
    "anim_fox_catch": (255, 183, 77),
}

VO = {
    "vo_who_says_hi": [392.0, 523.25],
    "vo_come_play_hint": [349.23, 440.0, 523.25],
    "vo_hi_speak": [523.25, 659.25],
    "vo_bye_expose": [392.0, 329.63, 261.63],
}

SFX_TONES = {
    "sfx_star": [523.25, 659.25, 783.99],
    "sfx_wrong_soft": [329.63, 261.63],
}

PNG_META = """fileFormatVersion: 2
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

WAV_META = """fileFormatVersion: 2
guid: {guid}
AudioImporter:
  externalObjects: {{}}
  serializedVersion: 7
  defaultSettings:
    serializedVersion: 2
    loadType: 0
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 0
    quality: 1
    conversionMode: 0
  platformSettingOverrides: {{}}
  forceToMono: 1
  normalize: 1
  preloadAudioData: 1
  loadInBackground: 0
  ambisonic: 0
  3D: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def write_png_meta(path):
    if os.path.exists(path + ".meta"):
        return
    with open(path + ".meta", "w") as f:
        f.write(PNG_META.format(guid=uuid.uuid4().hex, sprite=uuid.uuid4().hex))


def write_wav_meta(path):
    if os.path.exists(path + ".meta"):
        return
    with open(path + ".meta", "w") as f:
        f.write(WAV_META.format(guid=uuid.uuid4().hex))


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


def chip(w, h, rgb):
    r, g, b = rgb
    buf = bytearray(w * h * 4)
    cx, cy = w * 0.5, h * 0.5
    r2 = (w * 0.42) ** 2
    for y in range(h):
        for x in range(w):
            i = (y * w + x) * 4
            dx, dy = x + 0.5 - cx, y + 0.5 - cy
            if dx * dx + dy * dy <= r2:
                buf[i : i + 4] = bytes((r, g, b, 255))
            else:
                buf[i : i + 4] = bytes((0, 0, 0, 0))
    return buf


def wav(path, freqs, seconds=0.9, rate=22050):
    n = max(1, int(seconds * rate))
    frames = bytearray()
    for i in range(n):
        f = freqs[min(len(freqs) - 1, i * len(freqs) // n)]
        env = math.sin(math.pi * i / n)
        s = env * 0.28 * math.sin(2 * math.pi * f * i / rate)
        v = max(-1.0, min(1.0, s))
        frames += struct.pack("<h", int(v * 32767))
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with wave.open(path, "w") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(rate)
        w.writeframes(bytes(frames))


def main():
    for name, rgb in ANIM.items():
        buf = chip(256, 256, rgb)
        for folder in (ART, RES_ART):
            dest = os.path.join(folder, name + ".png")
            png(dest, 256, 256, buf)
            write_png_meta(dest)
            print("wrote", dest)
    for name, freqs in VO.items():
        for folder in (AUDIO, RES_AUDIO):
            dest = os.path.join(folder, name + ".wav")
            wav(dest, freqs)
            write_wav_meta(dest)
            print("wrote", dest)
    for name, freqs in SFX_TONES.items():
        dest = os.path.join(SFX, name + ".wav")
        wav(dest, freqs, seconds=0.4)
        write_wav_meta(dest)
        print("wrote", dest)
        dest = os.path.join(RES_SFX, name + ".wav")
        wav(dest, freqs, seconds=0.4)
        write_wav_meta(dest)
        print("wrote", dest)


if __name__ == "__main__":
    main()
