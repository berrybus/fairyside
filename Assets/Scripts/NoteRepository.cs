using System;

public struct NoteEntry {
    public string name;
    public string description;

    public NoteEntry(string name, string description) {
        this.name = name;
        this.description = description;
    }
}

public static class NoteRepository {
    public static NoteEntry[] monsterNotes = new NoteEntry[] {
        new NoteEntry(
            "Blue slime",
            "These creatures are everywhere on these grounds! Amorphous blobs without much intelligence, but they seem quite happy. They are native to Luninsula."
        ),
        new NoteEntry(
            "Green slime",
            "The first evolution of the blue slime, smaller and faster. Unsure if natural or man-made. Children often use the carcasses in games that require bouncy balls."
        ),
        new NoteEntry(
            "Big red slime",
            "Their numbers have dwindled for years on account of a rumor that their red cores are of delicate flavor bordering on the divine. Those who have tasted the core (and the amalgamation of human flesh rotting within) know better."
        ),
        new NoteEntry(
            "Pink slime",
            "Curious little things. They say a princess was once imprisoned in the dungeons. Her affinity for natural creatures extended to slimes. Each one of them has a ribbon tied personally by the princess herself!"
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
        new NoteEntry(
            "Placeholder name",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        ),
    };

    public static NoteEntry[] loreNotes = new NoteEntry[] {
        new NoteEntry(
            "My head",
            "It hurts it hurts it hurts it hurts it hurts it hurts it hurts it hurts it hurts it hurts it hurts it hurts it hurts it hurts it hurts it hurts"
        ),
        new NoteEntry(
            "Headmaster Fulligan",
            "A highly accomplished healer who quickly rose through the ranks of Redwood Academy. A small, kind man whose distinctive features were his large hawthorne glasses. He was known to occasionally lose confidence in himself."
        ),
        new NoteEntry(
            "Shirley's father",
            "A very hardworking man who raised three children. He always wanted to go to Redwood Academy but could neither pass the entrance exam nor obtain the funds to enroll. He died in the last plague."
        ),
        new NoteEntry(
            "Lunites",
            "The core of the magic in Luninsula. They are embedded in small rocks scattered throughout the island. Are we slowly draining the island of energy every time we use lunites?"
        ),
        new NoteEntry(
            "Lunite (II)",
            "Lunites are most often used by fairies to perform magic. Some fairies are better at channeling the power. Monsters will also injest lunites to make themselves stronger, but the gems are retrievable upon death."
        ),
        new NoteEntry(
            "Headaches",
            "A malaise affecting everyone on this awful, awful island. Please, someone, find a cure! I can't last much longer!"
        ),
        new NoteEntry(
            "Plague",
            "Every so often, disease sweeps across the island and leaves devastation in its wake. Despite our healers' best efforts, the sickness always kills. Perhaps nature is telling us we have overstepped our bounds."
        ),
        new NoteEntry(
            "Witches",
            "They say that the witches are the remnants of the old fairies who used to inhabit this island hundreds of years ago. Many people say witches are a myth, but I've seen them in the forests with my own eyes!"
        ),
        new NoteEntry(
            "Witches (II)",
            "According to school legend, Headmaster Fulligan once slew a witch. Unfortunately for us, witches are vengeful creatures."
        ),
        new NoteEntry(
            "Redwood Academy",
            "One of the first institutions established after the great Fairy-Human War. It is the dream of many Luninsula residents to attend, for the art of magic has been lost to many on the island."
        ),
        new NoteEntry(
            "Redwood Academy (II)",
            "It was created over the vestiges of another, more ancient institution, likely a dwelling-place of the original fairy civilization."
        ),
        new NoteEntry(
            "The Fairy-Human War",
            "Luninsula used to be inhabited only by fairies. However, humans, in their wretched ways, discovered the island and tried to take the precious resource of magic by force. It is said that the fairies bravely defended themselves."
        ),
        new NoteEntry(
            "Humans",
            "Vile creatures whose magic usage drains the island of power. The books say that all the humans were pushed out of the island after the war, but I don't believe that."
        ),
        new NoteEntry(
            "Fairies",
            "A breed that sprung out of Luninsula's natural well of magic. We live in harmony with our land and our magic practice replenishes the island with health."
        ),
        new NoteEntry(
            "Magic",
            "A mysterious force that helps us create miracles. It only exists on Luninsula, but the magic on the island is getting weaker, and nobody is willing to admit it."
        ),
        new NoteEntry(
            "Fairyside",
            "Only found in a couple of books, this spell will supposedly reveal all the world's knowledge to you and cure you of any ailments. The process to create it is extremely complicated and dangerous."
        ),
        new NoteEntry(
            "Love",
            "Some say that the real source of our magic lies not in lunites but in love."
        ),
        new NoteEntry(
            "Speedrunning",
            "I was thinking about why so many in the radical left participate in \'speedrunning\'. The reason is the left's lack of work ethic (\'go fast\' rather than \'do it right\') and, in a Petersonian sense, to elevate alternative sexual archetypes in the marketplace (\'fastest mario\')"
        )
    };
}