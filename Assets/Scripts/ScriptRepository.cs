using System;

public struct ScriptLine {
    public string line;
    public string name;
    public string avatar;

    public ScriptLine(string avatar, string name, string line) {
        this.avatar = avatar;
        this.name = name;
        this.line = line;
    }
}
public static class ScriptRepository
{
    static ScriptLine[] script0 = new ScriptLine[] {
        new ScriptLine(
            "",
            "",
            "For some reason, tonight I have decided to take a walk in the forest,"),
        new ScriptLine(
            "",
            "",
            "the very forest that we teachers forbid our students from entering."),
        new ScriptLine(
            "",
            "",
            "I remember the old stories they used to tell us about evil spirits hidden inside the dark trees"),
        new ScriptLine(
            "",
            "",
            "and how they trapped anyone foolish enough to wander in."),
        new ScriptLine(
            "",
            "",
            "But at the same time, this is also the most beautiful place in Luninsula,"),
        new ScriptLine(
            "",
            "",
            "for where else can you find such a lovely view of the moon perfectly framed by the gently swaying willow trees?"),
        new ScriptLine(
            "",
            "",
            "And yet there is something nefarious about the overbearing presence of the moon, whose perfection hides away a thousand sins."),
        new ScriptLine(
            "",
            "",
            "On nights like this, images of a time long gone flood my mind, and once again, I lose myself in the past."),
    };

    static ScriptLine[] script1 = new ScriptLine[] {
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "The first day of school! I'm so excited!"),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Ah, the bell!"),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Let's see, where's my first class..."),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Hmm, maybe I'll just follow those girls, they look like they know where they're going."),
        new ScriptLine(
            "",
            "Professor",
            "Attention, class! Please welcome our new student, Shirley."),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "H-hello everyone! My name is Shirley. I want to learn all sorts of magic with you all!"),
        new ScriptLine(
            "",
            "Professor",
            "You may take your seat, Shirley."),
        new ScriptLine(
            "",
            "",
            "The professor started lecturing, and I eagerly took notes."),
        new ScriptLine(
            "",
            "Professor",
            "When you begin the game, you can use WASD to move and the arrow keys to shoot."),
        new ScriptLine(
            "",
            "Professor",
            "Furthermore, you can press space or enter to advance the text, which you might already know!"),
        new ScriptLine(
            "",
            "Professor",
            "Press ESC to access the in-game menu at any time."),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Wait a minute, what does any of this have to do with magic?"),
        new ScriptLine(
            "",
            "",
            "The professor eventually stopped talking about strange key combinations and returned to what I had been expecting,"),
        new ScriptLine(
            "",
            "",
            "the principles of magic and other basics suited for a day one audience."),
        new ScriptLine(
            "",
            "",
            "He then moved on to a brief history of Luninsula"),
        new ScriptLine(
            "",
            "",
            "including the heroic leadership of Fay Viviane and the mysterious guardian spirits of the island."),
        new ScriptLine(
            "",
            "",
            "[.................]"),
        new ScriptLine(
            "",
            "Professor",
            "That's your lecture for today!"),
        new ScriptLine(
            "",
            "Professor",
            "Watch out for slimes in the hallways on your way back."),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "That was a long lecture!"),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "But it was kind of fun."),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "I hope I can actually cast all those cool spells the professor talked about..."),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Those girls again!"),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Hi! My name is Shirley, and I'm a new student!"),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Let's be friends."),
        new ScriptLine(
            "",
            "Girl A",
            "I know. You already told us your name. Like three hours ago."),
        new ScriptLine(
            "",
            "Girl B",
            "Hiya Shirley."),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "..."),
        new ScriptLine(
            "",
            "Girl A",
            "Okay, we're going to our next class. Have fun."),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "........"),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Maybe I need to take a class on my conversation skills..."),
        new ScriptLine(
            "ShirleyAvatar",
            "Shirley",
            "Ack, a monster...!"),
    };

    static ScriptLine[] script2 = new ScriptLine[] {
        new ScriptLine(
            "",
            "Professor",
            "Good afternoon, students!"),
        new ScriptLine(
            "",
            "Professor",
            "As you all know, there are all sorts of dangerous creatures in Luninsula,"),
        new ScriptLine(
            "",
            "Professor",
            "and today, you will learn to defend yourselves with attack spells."),
        new ScriptLine(
            "",
            "Student",
            "Finally!"),
        new ScriptLine(
            "",
            "Student",
            "I've been waiting for this..."),
        new ScriptLine(
            "",
            "",
            "The professor started lecturing as usual."),
        new ScriptLine(
            "",
            "",
            "Now I know that what he taught us were rather elementary techniques, but at the time,"),
        new ScriptLine(
            "",
            "",
            "I eagerly took notes from every word that came out of his mouth."),
        new ScriptLine(
            "",
            "",
            "[...]"),
        new ScriptLine(
            "",
            "",
            "[...]"),
        new ScriptLine(
            "",
            "Professor",
            "Now enough of that theory, let's go outside and put it into practice!"),
    };

    public static ScriptLine[][] scripts = new ScriptLine[][] { script0, script1, script2 };
}
