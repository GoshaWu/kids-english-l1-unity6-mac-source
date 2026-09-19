#!/usr/bin/env bash
# Pack a Mac-ready source zip of the Unity project (no Library/Temp/Obj/Logs).
# Usage: tools/pack_source_zip.sh [output-zip-path]
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
NAME="KidsEnglishL1-Unity6000.6.1f1-source"

if [[ -n "${1:-}" ]]; then
  DEST="$1"
elif [[ -d /opt/cursor/artifacts ]]; then
  DEST="/opt/cursor/artifacts/${NAME}.zip"
else
  DEST="/tmp/${NAME}.zip"
fi

mkdir -p "$(dirname "$DEST")"
cd "$ROOT"

if ! git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  echo "pack_source_zip.sh expects a git checkout (uses git archive)." >&2
  exit 1
fi

if [[ -n "$(git status --porcelain)" ]]; then
  echo "Working tree is dirty. Commit (or stash) before packing so the zip matches git." >&2
  exit 1
fi

# git archive emits tracked files only. Unity.gitignore already drops
# Library/, Temp/, Obj/, Logs/, Builds/, and UserSettings/.
git archive --format=zip --prefix="${NAME}/" -o "$DEST" HEAD

echo "Wrote $DEST"
unzip -l "$DEST" | awk '
  /Library\/|\/Temp\/|\/Obj\/|\/Logs\/|\/Builds\// { bad=1; print "ERROR: archive contains generated folder:", $0 }
  END { if (bad) exit 2 }
'
echo "Contents (top level):"
unzip -l "$DEST" | awk '{print $4}' | awk -F/ 'NF==2 && $2=="" {print $1}' | sort -u
ls -lh "$DEST"
