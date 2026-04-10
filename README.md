# PoiStorageRescue

A 7 Days to Die mod that drops player storage contents into loot bags when a POI reset would otherwise delete them.

This repository was generated from `Gluck-House/7dtd-mod-template`.

## Repository layout

- `PoiStorageRescue/`: solution, project file, source code, and packaged mod resources
- `PoiStorageRescue/src/`: the mod source tree
- `deps/`: local-only 7 Days to Die assemblies used for compilation
- `scripts/`: local developer helpers such as downloading pinned 7DTD dependencies
- `.github/7dtd-version.env`: the pinned 7DTD build used by CI
- `.github/release-please-config.json`: release automation settings for the mod repository
- `.github/release-please-manifest.json`: the current released mod version tracked by `release-please`
- `build.sh`: the standard local build entry point
- `CHANGELOG.md`: managed release notes for the mod repository
- `version.txt`: the repo-local mod version managed by `release-please`

## Building

Requirements:

- .NET SDK 8.0 or greater
- a copy of the 7 Days to Die dedicated server assemblies

The project expects these local game references in `deps/`:

- `0Harmony.dll`
- `Assembly-CSharp.dll`
- `LogLibrary.dll`
- `UnityEngine.dll`
- `UnityEngine.CoreModule.dll`

See [deps/README.md](deps/README.md) for the supported ways to populate that folder.

From the repo root:

```bash
./build.sh
```

If you want to invoke MSBuild directly instead:

```bash
dotnet build PoiStorageRescue/PoiStorageRescue.sln
```

The local build output is written to `PoiStorageRescue/build/PoiStorageRescue/`.

## CI behavior

CI pins the expected game dependency set using `.github/7dtd-version.env`.

- `build.yml` downloads the matching shared dependency bundle for the pinned build, builds the project, and uploads a ready-to-install artifact containing a top-level `PoiStorageRescue/` folder.
- `pr-title.yml` validates pull request titles against Conventional Commit formatting.
- `release-please.yml` maintains the long-running release PR, creates GitHub releases from merged release PRs, and uploads the packaged release artifact directly after release creation.
- `release.yml` builds the project from the same pinned dependency bundle and attaches a zip containing `PoiStorageRescue/` to the GitHub release.
- the normal pinned-build update loop runs centrally from `7dtd-mod-infra`, which publishes the matching dependency bundle and opens a PR when `.github/7dtd-version.env` should move forward

## Releases

- Pull request titles should use Conventional Commit formatting because squash merges drive release automation.
- `release-please` manages `CHANGELOG.md`, `version.txt`, and the mod version in `PoiStorageRescue/resources/ModInfo.xml`.
- Mod release tags use the `v<version>` format.
- `chore` entries appear in the generated changelog under `Maintenance`, but they do not trigger a release PR by themselves.

## Installation

1. Build the mod locally or download a CI/release artifact.
2. Copy the `PoiStorageRescue/` folder into your server `Mods/` directory.
3. Add whatever additional source, resources, and runtime configuration your mod needs.

## License

This repository is licensed under the terms in [LICENSE](LICENSE).
