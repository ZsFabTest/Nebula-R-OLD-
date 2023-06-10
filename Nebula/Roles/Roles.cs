using Nebula.Roles.Perk;

namespace Nebula.Roles;

static public class Roles
{
    public class SideCommonRoles
    {
        public Side side;
        public Role templateRole;

        public AllSideRoles.Secret? Secret;

        public SideCommonRoles(Side side, Role templateRole)
        {
            if (side == Side.Crewmate || side == Side.Impostor) Secret = new AllSideRoles.Secret(templateRole);

            SideRoles.Add(side, this);
        }

        public class SideCommonRolesLoader
        {
            public SideCommonRolesLoader()
            {
                new SideCommonRoles(Side.Crewmate, Roles.Crewmate);
                new SideCommonRoles(Side.Impostor, Roles.Impostor);
            }
        }

    }

    public static Dictionary<Side, SideCommonRoles> SideRoles = new Dictionary<Side, SideCommonRoles>();

    public static List<ExtraAssignable> AllExtraAssignable = new List<ExtraAssignable>();

    public static MetaRoles.VOID VOID = new MetaRoles.VOID();

    public static CrewmateRoles.Crewmate Crewmate = new CrewmateRoles.Crewmate();
    public static CrewmateRoles.CrewmateWithoutTasks CrewmateWithoutTasks = new CrewmateRoles.CrewmateWithoutTasks();
    public static CrewmateRoles.Agent Agent = new CrewmateRoles.Agent();
    public static CrewmateRoles.Alien Alien = new CrewmateRoles.Alien();
    public static CrewmateRoles.Bait Bait = new CrewmateRoles.Bait();
    public static CrewmateRoles.Busker Busker = new CrewmateRoles.Busker();
    public static CrewmateRoles.Comet Comet = new CrewmateRoles.Comet();
    public static CrewmateRoles.DamnedCrew DamnedCrew = new CrewmateRoles.DamnedCrew();
    public static CrewmateRoles.Doctor Doctor = new CrewmateRoles.Doctor();
    public static CrewmateRoles.Guardian Guardian = new CrewmateRoles.Guardian();
    public static CrewmateRoles.Mayor Mayor = new CrewmateRoles.Mayor();
    public static CrewmateRoles.Necromancer Necromancer = new CrewmateRoles.Necromancer();
    public static ComplexRoles.Guesser NiceGuesser = new ComplexRoles.Guesser("NiceGuesser", "niceGuesser", false);
    public static ComplexRoles.Tracker NiceTracker = new ComplexRoles.Tracker("NiceTracker", "niceTracker", false);
    public static ComplexRoles.Trapper NiceTrapper = new ComplexRoles.Trapper("NiceTrapper", "niceTrapper", false);
    public static CrewmateRoles.Oracle Oracle = new CrewmateRoles.Oracle();
    public static CrewmateRoles.Provocateur Provocateur = new CrewmateRoles.Provocateur();
    public static CrewmateRoles.Psychic Psychic = new CrewmateRoles.Psychic();
    public static CrewmateRoles.Navvy Navvy = new CrewmateRoles.Navvy();
    public static CrewmateRoles.Seer Seer = new CrewmateRoles.Seer();
    public static CrewmateRoles.Sheriff Sheriff = new CrewmateRoles.Sheriff();
    public static CrewmateRoles.Splicer Splicer = new CrewmateRoles.Splicer();
    public static CrewmateRoles.Spy Spy = new CrewmateRoles.Spy();

    public static CrewmateRoles.Madmate Madmate = new CrewmateRoles.Madmate();

    public static ImpostorRoles.Impostor Impostor = new ImpostorRoles.Impostor();
    public static ImpostorRoles.Banshee Banshee = new ImpostorRoles.Banshee();
    public static ImpostorRoles.BountyHunter BountyHunter = new ImpostorRoles.BountyHunter();
    public static ImpostorRoles.Camouflager Camouflager = new ImpostorRoles.Camouflager();
    public static ImpostorRoles.Cleaner Cleaner = new ImpostorRoles.Cleaner();
    public static ImpostorRoles.Covert Covert = new ImpostorRoles.Covert();
    public static ImpostorRoles.Damned Damned = new ImpostorRoles.Damned();
    public static ImpostorRoles.Disturber Disturber = new ImpostorRoles.Disturber();
    public static ImpostorRoles.Eraser Eraser = new ImpostorRoles.Eraser();
    public static ImpostorRoles.EvilAce EvilAce = new ImpostorRoles.EvilAce();
    public static ComplexRoles.Guesser EvilGuesser = new ComplexRoles.Guesser("EvilGuesser", "evilGuesser", true);
    public static ComplexRoles.Tracker EvilTracker = new ComplexRoles.Tracker("EvilTracker", "evilTracker", true);
    public static ComplexRoles.Trapper EvilTrapper = new ComplexRoles.Trapper("EvilTrapper", "evilTrapper", true);
    public static ImpostorRoles.Executioner Executioner = new ImpostorRoles.Executioner();
    public static ImpostorRoles.Jailer Jailer = new ImpostorRoles.Jailer();
    public static ImpostorRoles.Marionette Marionette = new ImpostorRoles.Marionette();
    public static ImpostorRoles.Morphing Morphing = new ImpostorRoles.Morphing();
    public static ImpostorRoles.Ninja Ninja = new ImpostorRoles.Ninja();
    public static ImpostorRoles.Painter Painter = new ImpostorRoles.Painter();
    public static ImpostorRoles.Raider Raider = new ImpostorRoles.Raider();
    public static ImpostorRoles.Reaper Reaper = new ImpostorRoles.Reaper();
    public static ImpostorRoles.Sniper Sniper = new ImpostorRoles.Sniper();

    public static NeutralRoles.Arsonist Arsonist = new NeutralRoles.Arsonist();
    public static NeutralRoles.Avenger Avenger = new NeutralRoles.Avenger();
    public static NeutralRoles.ChainShifter ChainShifter = new NeutralRoles.ChainShifter();
    public static NeutralRoles.Empiric Empiric = new NeutralRoles.Empiric();
    public static NeutralRoles.Immoralist Immoralist = new NeutralRoles.Immoralist();
    public static NeutralRoles.Jackal Jackal = new NeutralRoles.Jackal();
    public static NeutralRoles.Sidekick Sidekick = new NeutralRoles.Sidekick();
    public static NeutralRoles.Jester Jester = new NeutralRoles.Jester();
    public static NeutralRoles.Paparazzo Paparazzo = new NeutralRoles.Paparazzo();
    public static NeutralRoles.Opportunist Opportunist = new NeutralRoles.Opportunist();
    public static NeutralRoles.Spectre Spectre = new NeutralRoles.Spectre();
    //public static NeutralRoles.SantaClaus SantaClaus = new NeutralRoles.SantaClaus();
    //public static NeutralRoles.BlackSanta BlackSanta = new NeutralRoles.BlackSanta();
    public static NeutralRoles.Vulture Vulture = new NeutralRoles.Vulture();

    public static MinigameRoles.Player Player = new MinigameRoles.Player();


    public static MinigameRoles.Hunters.Hadar Hadar = new MinigameRoles.Hunters.Hadar();

    public static HnSCrewmateRoles.Crewmate HnSCrewmate = new HnSCrewmateRoles.Crewmate();

    public static HnSImpostorRoles.HnSCleaner HnSCleaner = new HnSImpostorRoles.HnSCleaner();
    public static HnSImpostorRoles.HnSHadar HnSHadar = new HnSImpostorRoles.HnSHadar();
    public static HnSImpostorRoles.HnSRaider HnSRaider = new HnSImpostorRoles.HnSRaider();
    public static HnSImpostorRoles.HnSReaper HnSReaper = new HnSImpostorRoles.HnSReaper();

    public static ComplexRoles.FCrewmate F_Crewmate = new ComplexRoles.FCrewmate();
    public static ComplexRoles.FGuesser F_Guesser = new ComplexRoles.FGuesser();
    public static ComplexRoles.FTracker F_Tracker = new ComplexRoles.FTracker();
    public static ComplexRoles.FTrapper F_Trapper = new ComplexRoles.FTrapper();

    public static ComplexRoles.SecondaryGuesser SecondaryGuesser = new ComplexRoles.SecondaryGuesser();
    public static NeutralRoles.SecondarySidekick SecondarySidekick = new NeutralRoles.SecondarySidekick();
    public static CrewmateRoles.SecondaryMadmate SecondaryMadmate = new CrewmateRoles.SecondaryMadmate();
    public static ImpostorRoles.Jailer.InheritedJailer InheritedJailer = new ImpostorRoles.Jailer.InheritedJailer();
    public static ExtraRoles.DiamondPossessor DiamondPossessor = new ExtraRoles.DiamondPossessor();
    public static ExtraRoles.Bloody Bloody = new ExtraRoles.Bloody();
    public static ExtraRoles.Confused Confused = new ExtraRoles.Confused();
    public static ExtraRoles.Drunk Drunk = new ExtraRoles.Drunk();
    public static ExtraRoles.LastImpostor LastImpostor = new ExtraRoles.LastImpostor();
    public static ExtraRoles.Lover Lover = new ExtraRoles.Lover();
    public static ExtraRoles.Trilemma Trilemma = new ExtraRoles.Trilemma();
    public static ExtraRoles.AvengerTarget AvengerTarget = new ExtraRoles.AvengerTarget();
   // public static ExtraRoles.TeamSanta TeamSanta = new ExtraRoles.TeamSanta();
    public static MetaRoles.MetaRole MetaRole = new MetaRoles.MetaRole();
    public static PerkHolder PerkHolder = new Perk.PerkHolder();

    public static GhostRoles.Poltergeist Poltergeist = new GhostRoles.Poltergeist();

    public static AllSideRoles.GameMaster GameMaster = new AllSideRoles.GameMaster();
    public static ComplexRoles.FDecider F_Decider = new ComplexRoles.FDecider();
    public static ComplexRoles.Decider EvilDecider = new ComplexRoles.Decider("EvilDecider", "evilDecider", true);
    public static ComplexRoles.Decider NiceDecider = new ComplexRoles.Decider("NiceDecider", "niceDecider", false);
    public static CrewmateRoles.SecondaryBait SecondaryBait = new CrewmateRoles.SecondaryBait();
    public static CrewmateRoles.Holmes Holmes = new CrewmateRoles.Holmes();
    public static CrewmateRoles.Kotwal Kotwal = new CrewmateRoles.Kotwal();
    public static CrewmateRoles.Necrophilic Necrophilic = new CrewmateRoles.Necrophilic();
    public static CrewmateRoles.Observer Observer = new CrewmateRoles.Observer();
    public static CrewmateRoles.OnlineCelebrity OnlineCelebrity = new CrewmateRoles.OnlineCelebrity();
    public static CrewmateRoles.Sanctifier Sanctifier = new CrewmateRoles.Sanctifier();
    //public static CrewmateRoles.Transporter Transporter = new CrewmateRoles.Transporter();
    public static CrewmateRoles.WhiteCat WhiteCat = new CrewmateRoles.WhiteCat();
    public static ExtraRoles.Flash Flash = new ExtraRoles.Flash();
    public static ImpostorRoles.Assassin Assassin = new ImpostorRoles.Assassin();
    public static ImpostorRoles.Demagogue Demagogue = new ImpostorRoles.Demagogue();
    public static ImpostorRoles.Escapist Escapist = new ImpostorRoles.Escapist();
    public static ImpostorRoles.EvilBusker EvilBusker = new ImpostorRoles.EvilBusker();
    public static ImpostorRoles.RedCat RedCat = new ImpostorRoles.RedCat();
    public static ImpostorRoles.SerialKiller SerialKiller = new ImpostorRoles.SerialKiller();
    public static ImpostorRoles.Vampire Vampire = new ImpostorRoles.Vampire();
    public static NeutralRoles.BlueCat BlueCat = new NeutralRoles.BlueCat();
    public static NeutralRoles.Madman Madman = new NeutralRoles.Madman();
    public static NeutralRoles.SchrodingersCat SchrodingersCat = new NeutralRoles.SchrodingersCat();
    public static NeutralRoles.Pavlov Pavlov = new NeutralRoles.Pavlov();
    public static NeutralRoles.Dog Dog = new NeutralRoles.Dog();
    public static CrewmateRoles.Dictator Dictator = new CrewmateRoles.Dictator();
    public static NeutralRoles.JackalMayor JackalMayor = new NeutralRoles.JackalMayor();
    public static NeutralRoles.PavlovsCat PavlovsCat = new NeutralRoles.PavlovsCat();
    public static ImpostorRoles.InvisibleMan InvisibleMan = new ImpostorRoles.InvisibleMan();
    public static NeutralRoles.Moriarty Moriarty = new NeutralRoles.Moriarty();
    public static NeutralRoles.Moran Moran = new NeutralRoles.Moran();
    public static NeutralRoles.Cascrubinter Cascrubinter = new NeutralRoles.Cascrubinter();
    public static ImpostorRoles.Heretic Heretic = new ImpostorRoles.Heretic();
    public static NeutralRoles.Amnesiac Amnesiac = new NeutralRoles.Amnesiac();
    public static CrewmateRoles.Bartender Bartender = new CrewmateRoles.Bartender();
    public static ComplexRoles.FSwapper F_Swapper = new ComplexRoles.FSwapper();
    public static ComplexRoles.Swapper EvilSwapper = new ComplexRoles.Swapper("EvilSwapper","evilSwapper",true);
    public static ComplexRoles.Swapper NiceSwapper = new ComplexRoles.Swapper("NiceSwapper","niceSwapper",false);
    public static ExtraRoles.SecondaryJackal SecondaryJackal = new ExtraRoles.SecondaryJackal();
    public static CrewmateRoles.LuckyMan LuckyMan = new CrewmateRoles.LuckyMan();
    public static ImpostorRoles.Moda Moda = new ImpostorRoles.Moda();
    public static GhostRoles.GhostEngineer GhostEngineer = new GhostRoles.GhostEngineer();
    public static ImpostorRoles.Marksman Marksman = new ImpostorRoles.Marksman();
    public static ImpostorRoles.Gambler Gambler = new ImpostorRoles.Gambler();
    public static CrewmateRoles.Superstar Superstar = new CrewmateRoles.Superstar();
    public static CrewmateRoles.ChivalrousExpert ChivalrousExpert = new CrewmateRoles.ChivalrousExpert();
    public static CrewmateRoles.Programmer Programmer = new CrewmateRoles.Programmer();
    public static ImpostorRoles.Plumber Plumber = new ImpostorRoles.Plumber();
    public static CrewmateRoles.Minekeeper Minekeeper = new CrewmateRoles.Minekeeper();
    public static ImpostorRoles.ChainKiller ChainKiller = new ImpostorRoles.ChainKiller();
    public static ImpostorRoles.Terrorist Terrorist = new ImpostorRoles.Terrorist();
    public static ImpostorRoles.Dying Dying = new ImpostorRoles.Dying();
    public static CrewmateRoles.Locksmith Locksmith = new CrewmateRoles.Locksmith();
    public static ExtraRoles.FakeLover FakeLover = new ExtraRoles.FakeLover();
    public static ExtraRoles.Cheater Cheater = new ExtraRoles.Cheater();
    public static ComplexRoles.FMover F_Mover = new ComplexRoles.FMover();
    public static ComplexRoles.Mover EvilMover = new ComplexRoles.Mover("EvilMover","evilMover",true);
    public static ComplexRoles.Mover NiceMover = new ComplexRoles.Mover("NiceMover","niceMover",false);
    public static ComplexRoles.FTransporter F_Transporter = new ComplexRoles.FTransporter();
    public static ComplexRoles.Transporter EvilTransporter = new ComplexRoles.Transporter("EvilTransporter","evilTransporter",true);
    public static ComplexRoles.Transporter NiceTransporter = new ComplexRoles.Transporter("NiceTransporter","niceTransporter",false);
    public static ImpostorRoles.Retarder Retarder = new ImpostorRoles.Retarder();
    public static NeutralRoles.Yandere Yandere = new NeutralRoles.Yandere();
    public static ExtraRoles.SecretCrush SecretCrush = new ExtraRoles.SecretCrush();
    public static ImpostorRoles.Marker Marker = new ImpostorRoles.Marker();

    //全てのロールはこの中に含まれている必要があります
    public static List<Role> AllRoles = new List<Role>()
        {
            VOID,
            Impostor,Banshee,BountyHunter,Camouflager,Cleaner,Covert,Damned,Disturber,Eraser,EvilAce,EvilGuesser,EvilTracker,EvilTrapper,Executioner,Jailer,Marionette,Morphing,Ninja,Painter,Raider,Reaper,Sniper,
            /*SantaClaus,BlackSanta,*/Arsonist,Avenger,ChainShifter,Empiric,Immoralist,Jackal,Jester,Opportunist,Paparazzo,Sidekick,Spectre,Vulture,
            F_Crewmate,
            F_Guesser,F_Tracker,F_Trapper,
            Crewmate,CrewmateWithoutTasks,Agent,Alien,Bait,Busker,Comet,DamnedCrew,Doctor,Guardian,Mayor,Navvy,Necromancer,NiceGuesser,NiceTracker,NiceTrapper,Oracle,Provocateur,Psychic,Seer,Sheriff,Splicer,Spy,
            Madmate,

            GameMaster,F_Decider,EvilDecider,NiceDecider,F_Swapper,EvilSwapper,NiceSwapper,F_Mover,EvilMover,NiceMover,F_Transporter,EvilTransporter,NiceTransporter,
            Holmes,Kotwal,Necrophilic,Observer,OnlineCelebrity,Sanctifier,/*Transporter,*/Dictator,Bartender,LuckyMan,Superstar,ChivalrousExpert,Programmer,Minekeeper,Locksmith,
            Assassin,Demagogue,Escapist,EvilBusker,SerialKiller,Vampire,InvisibleMan,Heretic,Moda,Marksman,Gambler,Plumber,ChainKiller,Terrorist,Dying,Retarder,Marker,
            Madman,Pavlov,Dog,Moriarty,Moran,Cascrubinter,Amnesiac,Yandere,
            JackalMayor,
            SchrodingersCat,RedCat,BlueCat,WhiteCat,PavlovsCat,

            Player,
            Hadar,
            HnSCrewmate,
            HnSCleaner,HnSHadar,HnSRaider,HnSReaper
        };

    public static List<ExtraRole> AllExtraRoles = new List<ExtraRole>()
        {
            SecondaryGuesser,SecondarySidekick,SecondaryMadmate,InheritedJailer,
            DiamondPossessor,LastImpostor,/*TeamSanta,*/
            Bloody,Confused,Drunk,Lover,Trilemma,
            MetaRole,AvengerTarget,
            PerkHolder,
            SecondaryBait,Flash,SecondaryJackal,FakeLover,Cheater,SecretCrush
        };

    public static List<GhostRole> AllGhostRoles = new List<GhostRole>()
        {
            Poltergeist,GhostEngineer
        };

    private static SideCommonRoles.SideCommonRolesLoader loader = new SideCommonRoles.SideCommonRolesLoader();

    public static void ResetWinTrigger()
    {
        foreach (Role role in AllRoles)
        {
            if (role is Template.HasWinTrigger)
            {
                ((Template.HasWinTrigger)role).WinTrigger = false;
            }
        }

        foreach (ExtraRole role in AllExtraRoles)
        {
            if (role is Template.HasWinTrigger)
            {
                ((Template.HasWinTrigger)role).WinTrigger = false;
            }
        }
    }

    public static void StaticInitialize()
    {
        foreach (Role role in AllRoles)
        {
            role.StaticInitialize();
        }

        foreach (ExtraRole role in AllExtraRoles)
        {
            role.StaticInitialize();
        }

        foreach (Role role in AllRoles)
        {
            role.OnRoleRelationSetting();
        }
    }
}
