# GuJian3Manager

Tools for GuJian 3 files.

Data files are compressed with oodle. You need to copy `oo2core_6_win64.dll` (included in the game) into the app folder.

.xxx files are encrypted with XXTEA encryption.

## Building the app

```
git clone https://github.com/Kaplas80/GuJian3Manager.git

cd GuJian3Manager/src

dotnet build
```

## Usage

### Show data files info

```
GuJian3Tool.exe info <path_to_303.idx>
```

### Extract all data files

```
GuJian3Tool.exe extract <path_to_303.idx> <output_directory>
```

### Extract single data file

```
GuJian3Tool.exe extract-single <path_to_dataXXX> <output_directory> [--index <path_to_303.idx>]
```

Index path is optional, but if you don't use it, the files will be extracted without readable name.

### Add / Replace files in data files

```
GuJian3Tool.exe build <path_to_new_files> <path_to_303.idx>
```

The new files have to have the full path to work properly. For example: to replace the english fonts in game, you have to place the files in:
`<some_path>\asset\interface\Resource_en\font`
and execute the app using:
`GuJian3Tool.exe build some_path path\to\303.idx`

### Decrypt file

```
GuJian3Tool.exe decrypt <path_to_xxx_file> <output_file> [--key encryption_key]
```

Encryption key is optional.

These 4 files has a unknown encryption key, so they can not be decrypted:

- asset\maps\m24\elems.xxx
- asset\interface\Resource\movie\EDtxt.xxx
- asset\interface\Resource\movie\OPtxt.xxx
- asset\interface\Resource_cht\movie\EDtxt.xxx

### Encrypt file

```
GuJian3Tool.exe encrypt <input_file> <output_file> [--key encryption_key]
```

Encryption key is optional.

### Extract game data to JSON

```
GuJian3Tool.exe extract-json <path_to_text.bin> <output_json_file>
```

NOTE: The input file is a decrypted exe section. See alanm [mod loader](https://github.com/alanm20/Gujian3TextMod).

### Build game data from JSON

```
GuJian3Tool.exe build-json <input_json_file> <output_file>
```

### Extract game strings to PO

```
GuJian3Tool.exe extract-text <path_to_text.bin> <output_directory>
```

NOTE: The input file is a decrypted exe section. See alanm [mod loader](https://github.com/alanm20/Gujian3TextMod).

### Replace strings in game data

```
GuJian3Tool.exe build-text <path_to_text.bin> <po_directory> <output_file>
```

NOTE: The input file is a decrypted exe section. See alanm [mod loader](https://github.com/alanm20/Gujian3TextMod).

## Credits

- Thanks to Pleonex for [Yarhl](https://scenegate.github.io/Yarhl/) and for identifying XXTEA algorithm.
- Thanks to DARKSiDERS for finding all the encryption keys.
- Thanks to JKAnderson for the [OodleWrapper](https://github.com/JKAnderson/SoulsFormats/blob/master/SoulsFormats/Util/Oodle26.cs).
- Thanks to eprilx for the [Gujian3TextEditor](https://github.com/eprilx/Gujian3TextEditor).
- Thanks to alanm for the [mod loader](https://github.com/alanm20/Gujian3TextMod).
