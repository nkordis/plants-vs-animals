#!/usr/bin/env python3
"""Fail if any 64-bit native library in an .aab/.apk has a LOAD segment aligned below 16 KB.

Google Play requires 16 KB page-size support for apps targeting Android 15+.
Usage: check_16kb_alignment.py <path-to-aab-or-apk>
"""
import struct
import sys
import zipfile

MIN_ALIGN = 16 * 1024
PT_LOAD = 1


def load_alignments(data):
    if data[:4] != b"\x7fELF" or data[4] != 2:  # only 64-bit ELF matters for 16 KB devices
        return None
    endian = "<" if data[5] == 1 else ">"
    e_phoff = struct.unpack_from(endian + "Q", data, 0x20)[0]
    e_phentsize, e_phnum = struct.unpack_from(endian + "HH", data, 0x36)
    aligns = []
    for i in range(e_phnum):
        off = e_phoff + i * e_phentsize
        p_type = struct.unpack_from(endian + "I", data, off)[0]
        if p_type == PT_LOAD:
            aligns.append(struct.unpack_from(endian + "Q", data, off + 0x30)[0])
    return aligns


def main(path):
    failures, checked = [], 0
    with zipfile.ZipFile(path) as z:
        for name in z.namelist():
            if not name.endswith(".so"):
                continue
            aligns = load_alignments(z.read(name))
            if aligns is None:
                continue
            checked += 1
            low = [a for a in aligns if a < MIN_ALIGN]
            status = "FAIL" if low else "ok"
            print(f"{status:4} {name} min LOAD align {min(aligns)}")
            if low:
                failures.append(name)
    if checked == 0:
        print("No 64-bit native libraries found; is this the right file?")
        return 1
    if failures:
        print(f"{len(failures)} librar(ies) are not 16 KB aligned")
        return 1
    print(f"All {checked} 64-bit native libraries are 16 KB aligned")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1]))
