#!/bin/bash
# SessionStart hook: provision the .NET 10 SDK in a remote agent container.
#
# Remote containers start without a .NET SDK on PATH, so builds, tests and linters
# would fail. Local machines are expected to have it already. Idempotent.
set -euo pipefail

# Only provision in a remote container. REMOTE_AGENT is set by this kit's callers;
# CLAUDE_CODE_REMOTE is set by Claude Code on the web.
if [ "${REMOTE_AGENT:-${CLAUDE_CODE_REMOTE:-}}" != "true" ]; then
  exit 0
fi

if command -v dotnet >/dev/null 2>&1 && dotnet --list-sdks 2>/dev/null | grep -q '^10\.'; then
  echo "dotnet-sdk-10 already present: $(dotnet --version)"
  exit 0
fi

export DEBIAN_FRONTEND=noninteractive
echo "Installing dotnet-sdk-10.0 from the Ubuntu archive..."
apt-get update
apt-get install -y --no-install-recommends dotnet-sdk-10.0
echo "Installed .NET SDK: $(dotnet --version)"
