# AssParser &middot; [![Publish to nuget](https://github.com/AmusementClub/AssParser/actions/workflows/dotnet-nuget.yml/badge.svg)](https://github.com/AmusementClub/AssParser/actions/workflows/dotnet-nuget.yml) ![Nuget](https://img.shields.io/nuget/v/AssParser.Lib?logo=nuget)  [![Test](https://github.com/AmusementClub/AssParser/actions/workflows/test.yml/badge.svg)](https://github.com/AmusementClub/AssParser/actions/workflows/test.yml)

Parse ASS(SubStation Alpha Subtitles) file faster. No Regex. All managed code.

## Basic Parse

``` cs
AssSubtitleModel assfile = Lib.AssParser.ParseAssFile(@"path/to/your/assfile").Result;

# Or async way

AssSubtitleModel assfile = await Lib.AssParser.ParseAssFile(@"path/to/your/assfile");
```

## List used fonted
``` cs
AssSubtitleModel assfile = Lib.AssParser.ParseAssFile(@"path/to/your/assfile").Result;
FontDetail[] fonts = assfile.UsedFonts();
```
Where FontDetail is defined as
``` cs
public class FontDetail : IEquatable<FontDetail?>
{
    public string FontName = "";
    public string UsedChar = "";
    public int Bold;
    public bool IsItalic;

    public override bool Equals(object? obj)
    {
        return Equals(obj as FontDetail);
    }

    public bool Equals(FontDetail? other)
    {
        return other is not null &&
               FontName == other.FontName &&
               Bold == other.Bold &&
               IsItalic == other.IsItalic;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FontName, Bold, IsItalic);
    }

    public static bool operator ==(FontDetail? left, FontDetail? right)
    {
        return EqualityComparer<FontDetail>.Default.Equals(left, right);
    }

    public static bool operator !=(FontDetail? left, FontDetail? right)
    {
        return !(left == right);
    }
}
```

## Get extra section
``` cs
AssSubtitleModel assfile = Lib.AssParser.ParseAssFile(Path.Combine("UUEncodeTest", "1.ass")).Result;
string fontsData = assfile.UnknownSections["[Fonts]"];
```

## Decode & Encode UUEncode
``` cs
byte[] data = UUEncode.Decode(fontsData, out var crlf);
string encoded = UUEncode.Eecode(data, crlf)
