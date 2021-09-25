# GuJian3Manager

Tools for GuJian 3 files.

**ATTENTION: This project won't get more updates. My first goal was to find the game strings to be able to translate them, but they aren't located inside the data files.**

Data files are compressed with oodle. You need to copy `oo2core_6_win64.dll` (included in the game) into the app folder.

.xxx files are encrypted with XXTEA encryption.

## Building

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

### Extract data files

```
GuJian3Tool.exe extract <path_to_303.idx> <output_directory>
```

### Decrypt files

```
GuJian3Tool.exe decrypt <path_to_xxx_file> <output_file>
```

These 4 files has a unknown encryption key, so they can not be decrypted:

- asset\maps\m24\elems.xxx
- asset\interface\Resource\movie\EDtxt.xxx
- asset\interface\Resource\movie\OPtxt.xxx
- asset\interface\Resource_cht\movie\EDtxt.xxx

## Credits

- Thanks to Pleonex for [Yarhl](https://scenegate.github.io/Yarhl/) and for identifying XXTEA algorithm.
- Thanks to DARKSiDERS for finding all the encryption keys.
