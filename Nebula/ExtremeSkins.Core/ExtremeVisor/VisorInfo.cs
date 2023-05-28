namespace ExtremeSkins.Core.ExtremeVisor;

public record VisorInfo(
    string Name, string Author,
    bool LeftIdle = false,
    bool Shader = false,
    bool BehindHat = false,
    string ComitHash = "") : InfoBase(Name, Author);