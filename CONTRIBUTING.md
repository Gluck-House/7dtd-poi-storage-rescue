# Contributing

## Local setup

1. Populate `deps/` with the required 7 Days to Die assemblies.
2. Run `./build.sh` from the repository root.
3. Test the resulting mod from `PoiStorageRescue/build/PoiStorageRescue/`.

To download the required assemblies automatically:

```bash
STEAMCMD_MODE=docker ./scripts/download_7dtd_server.sh
```

## Notes

- `deps/`, `.cache/`, `.tools/`, and `PoiStorageRescue/build/` are local-only and ignored by git.
- CI uploads an artifact with a top-level `PoiStorageRescue/` folder for direct use in `Mods/`.
- Keep changes portable. Avoid absolute filesystem paths and machine-specific assumptions in docs or scripts.
- The project currently builds with .NET SDK 8 while targeting `netstandard2.1` for the game-facing assembly.
- This repository was generated from `Gluck-House/7dtd-mod-template`.

## Pull Request Titles

- Use Conventional Commit PR titles so squash merges produce clean release history.
- Typical examples: `feat: add reusable workflow wrapper`, `fix: correct dependency bundle path`, `chore(template): refresh Copier scaffold`.
- PR titles are validated in CI with `pr-title.yml`.
