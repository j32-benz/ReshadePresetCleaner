# 🧹✨ ReshadePresetCleaner ✨🧹

`ReshadePresetCleaner` is a tool to clean up ReShade preset files by removing unnecessary sections that were autosaved.

![🖼️ Screenshot](https://files.catbox.moe/93592b.png)

## 🛠️ Usage

```sh
ReshadePresetCleaner <path-to-ini-file> [--keep-layer]
```

- `<path-to-ini-file>`: 📂 Path to the ReShade `.ini` file to clean.
- `--keep-layer` (optional): 🛡️ If included, retains `Layer.fx` and its section in the output.

The cleaned file is saved in the same directory with the prefix `cleaned_`.

## 📜 License

This program is released under the MIT License. 🎉
