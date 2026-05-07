download-deps:
    #!/usr/bin/env bash
    OVERHAULLIB_VER=1.21.0
    mkdir -p lib/overhaullib
    cd lib/overhaullib
    curl -LO https://mods.vintagestory.at/download/78190/overhaullib_$OVERHAULLIB_VER.zip
    unzip overhaullib_$OVERHAULLIB_VER.zip
    rm overhaullib_$OVERHAULLIB_VER.zip
    cd -

download-vs:
    #!/usr/bin/env bash
    VS_DOWNLOAD_URL="https://cdn.vintagestory.at/gamefiles/stable/vs_client_linux-x64_1.22.2.tar.gz"
    curl -o vs.tar.gz $VS_DOWNLOAD_URL
    tar -xzf vs.tar.gz
    test -d vintagestory
