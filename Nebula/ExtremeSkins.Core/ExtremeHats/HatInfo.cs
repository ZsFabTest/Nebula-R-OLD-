namespace ExtremeSkins.Core.ExtremeHats;

public record HatInfo(
    string Name, string Author,
    bool Bound = false,
    bool Shader = false,
    bool Climb = false,
    bool FrontFlip = false,
    bool Back = false,
    bool BackFlip = false,
    string comitHash = "") : InfoBase(Name, Author);