using System.Reflection;
using TerminalGPT.Options;

namespace TerminalGPT.Constants;

public class AppConstants
{
    #region FigletFont

    public const string FigletFont = @"flf2a$ 7 7 13 0 7 0 64 0
Font Author: ?

More Info:

https://web.archive.org/web/20120819044459/http://www.roysac.com/thedrawfonts-tdf.asp

FIGFont created with: http://patorjk.com/figfont-editor
$  $@
$  $@
$  $@
$  $@
$  $@
$  $@
$  $@@
██╗@
██║@
██║@
╚═╝@
██╗@
╚═╝@
   @@
@
@
@
@
@
@
@@
 ██╗ ██╗ @
████████╗@
╚██╔═██╔╝@
████████╗@
╚██╔═██╔╝@
 ╚═╝ ╚═╝ @
         @@
▄▄███▄▄·@
██╔════╝@
███████╗@
╚════██║@
███████║@
╚═▀▀▀══╝@
        @@
██╗ ██╗@
╚═╝██╔╝@
  ██╔╝ @
 ██╔╝  @
██╔╝██╗@
╚═╝ ╚═╝@
       @@
   ██╗   @
   ██║   @
████████╗@
██╔═██╔═╝@
██████║  @
╚═════╝  @
         @@
@
@
@
@
@
@
@@
 ██╗@
██╔╝@
██║ @
██║ @
╚██╗@
 ╚═╝@
    @@
██╗ @
╚██╗@
 ██║@
 ██║@
██╔╝@
╚═╝ @
    @@
      @
▄ ██╗▄@
 ████╗@
▀╚██╔▀@
  ╚═╝ @
      @
      @@
@
@
@
@
@
@
@@
   @
   @
   @
   @
▄█╗@
╚═╝@
   @@
      @
      @
█████╗@
╚════╝@
      @
      @
      @@
   @
   @
   @
   @
██╗@
╚═╝@
   @@
    ██╗@
   ██╔╝@
  ██╔╝ @
 ██╔╝  @
██╔╝   @
╚═╝    @
       @@
 ██████╗ @
██╔═████╗@
██║██╔██║@
████╔╝██║@
╚██████╔╝@
 ╚═════╝ @
         @@
 ██╗@
███║@
╚██║@
 ██║@
 ██║@
 ╚═╝@
    @@
██████╗ @
╚════██╗@
 █████╔╝@
██╔═══╝ @
███████╗@
╚══════╝@
        @@
██████╗ @
╚════██╗@
 █████╔╝@
 ╚═══██╗@
██████╔╝@
╚═════╝ @
        @@
██╗  ██╗@
██║  ██║@
███████║@
╚════██║@
     ██║@
     ╚═╝@
        @@
███████╗@
██╔════╝@
███████╗@
╚════██║@
███████║@
╚══════╝@
        @@
 ██████╗ @
██╔════╝ @
███████╗ @
██╔═══██╗@
╚██████╔╝@
 ╚═════╝ @
         @@
███████╗@
╚════██║@
    ██╔╝@
   ██╔╝ @
   ██║  @
   ╚═╝  @
        @@
 █████╗ @
██╔══██╗@
╚█████╔╝@
██╔══██╗@
╚█████╔╝@
 ╚════╝ @
        @@
 █████╗ @
██╔══██╗@
╚██████║@
 ╚═══██║@
 █████╔╝@
 ╚════╝ @
        @@
   @
██╗@
╚═╝@
██╗@
╚═╝@
   @
   @@
   @
██╗@
╚═╝@
▄█╗@
▀═╝@
   @
   @@
  ██╗@
 ██╔╝@
██╔╝ @
╚██╗ @
 ╚██╗@
  ╚═╝@
     @@
@
@
@
@
@
@
@@
██╗  @
╚██╗ @
 ╚██╗@
 ██╔╝@
██╔╝ @
╚═╝  @
     @@
██████╗ @
╚════██╗@
  ▄███╔╝@
  ▀▀══╝ @
  ██╗   @
  ╚═╝   @
        @@
 ██████╗ @
██╔═══██╗@
██║██╗██║@
██║██║██║@
╚█║████╔╝@
 ╚╝╚═══╝ @
         @@
 █████╗ @
██╔══██╗@
███████║@
██╔══██║@
██║  ██║@
╚═╝  ╚═╝@
        @@
██████╗ @
██╔══██╗@
██████╔╝@
██╔══██╗@
██████╔╝@
╚═════╝ @
        @@
 ██████╗@
██╔════╝@
██║     @
██║     @
╚██████╗@
 ╚═════╝@
        @@
██████╗ @
██╔══██╗@
██║  ██║@
██║  ██║@
██████╔╝@
╚═════╝ @
        @@
███████╗@
██╔════╝@
█████╗  @
██╔══╝  @
███████╗@
╚══════╝@
        @@
███████╗@
██╔════╝@
█████╗  @
██╔══╝  @
██║     @
╚═╝     @
        @@
 ██████╗ @
██╔════╝ @
██║  ███╗@
██║   ██║@
╚██████╔╝@
 ╚═════╝ @
         @@
██╗  ██╗@
██║  ██║@
███████║@
██╔══██║@
██║  ██║@
╚═╝  ╚═╝@
        @@
██╗@
██║@
██║@
██║@
██║@
╚═╝@
   @@
     ██╗@
     ██║@
     ██║@
██   ██║@
╚█████╔╝@
 ╚════╝ @
        @@
██╗  ██╗@
██║ ██╔╝@
█████╔╝ @
██╔═██╗ @
██║  ██╗@
╚═╝  ╚═╝@
        @@
██╗     @
██║     @
██║     @
██║     @
███████╗@
╚══════╝@
        @@
███╗   ███╗@
████╗ ████║@
██╔████╔██║@
██║╚██╔╝██║@
██║ ╚═╝ ██║@
╚═╝     ╚═╝@
           @@
███╗   ██╗@
████╗  ██║@
██╔██╗ ██║@
██║╚██╗██║@
██║ ╚████║@
╚═╝  ╚═══╝@
          @@
 ██████╗ @
██╔═══██╗@
██║   ██║@
██║   ██║@
╚██████╔╝@
 ╚═════╝ @
         @@
██████╗ @
██╔══██╗@
██████╔╝@
██╔═══╝ @
██║     @
╚═╝     @
        @@
 ██████╗ @
██╔═══██╗@
██║   ██║@
██║▄▄ ██║@
╚██████╔╝@
 ╚══▀▀═╝ @
         @@
██████╗ @
██╔══██╗@
██████╔╝@
██╔══██╗@
██║  ██║@
╚═╝  ╚═╝@
        @@
███████╗@
██╔════╝@
███████╗@
╚════██║@
███████║@
╚══════╝@
        @@
████████╗@
╚══██╔══╝@
   ██║   @
   ██║   @
   ██║   @
   ╚═╝   @
         @@
██╗   ██╗@
██║   ██║@
██║   ██║@
██║   ██║@
╚██████╔╝@
 ╚═════╝ @
         @@
██╗   ██╗@
██║   ██║@
██║   ██║@
╚██╗ ██╔╝@
 ╚████╔╝ @
  ╚═══╝  @
         @@
██╗    ██╗@
██║    ██║@
██║ █╗ ██║@
██║███╗██║@
╚███╔███╔╝@
 ╚══╝╚══╝ @
          @@
██╗  ██╗@
╚██╗██╔╝@
 ╚███╔╝ @
 ██╔██╗ @
██╔╝ ██╗@
╚═╝  ╚═╝@
        @@
██╗   ██╗@
╚██╗ ██╔╝@
 ╚████╔╝ @
  ╚██╔╝  @
   ██║   @
   ╚═╝   @
         @@
███████╗@
╚══███╔╝@
  ███╔╝ @
 ███╔╝  @
███████╗@
╚══════╝@
        @@
███╗@
██╔╝@
██║ @
██║ @
███╗@
╚══╝@
    @@
@
@
@
@
@
@
@@
███╗@
╚██║@
 ██║@
 ██║@
███║@
╚══╝@
    @@
 ███╗ @
██╔██╗@
╚═╝╚═╝@
      @
      @
      @
      @@
        @
        @
        @
        @
███████╗@
╚══════╝@
        @@
@
@
@
@
@
@
@@
 █████╗ @
██╔══██╗@
███████║@
██╔══██║@
██║  ██║@
╚═╝  ╚═╝@
        @@
██████╗ @
██╔══██╗@
██████╔╝@
██╔══██╗@
██████╔╝@
╚═════╝ @
        @@
 ██████╗@
██╔════╝@
██║     @
██║     @
╚██████╗@
 ╚═════╝@
        @@
██████╗ @
██╔══██╗@
██║  ██║@
██║  ██║@
██████╔╝@
╚═════╝ @
        @@
███████╗@
██╔════╝@
█████╗  @
██╔══╝  @
███████╗@
╚══════╝@
        @@
███████╗@
██╔════╝@
█████╗  @
██╔══╝  @
██║     @
╚═╝     @
        @@
 ██████╗ @
██╔════╝ @
██║  ███╗@
██║   ██║@
╚██████╔╝@
 ╚═════╝ @
         @@
██╗  ██╗@
██║  ██║@
███████║@
██╔══██║@
██║  ██║@
╚═╝  ╚═╝@
        @@
██╗@
██║@
██║@
██║@
██║@
╚═╝@
   @@
     ██╗@
     ██║@
     ██║@
██   ██║@
╚█████╔╝@
 ╚════╝ @
        @@
██╗  ██╗@
██║ ██╔╝@
█████╔╝ @
██╔═██╗ @
██║  ██╗@
╚═╝  ╚═╝@
        @@
██╗     @
██║     @
██║     @
██║     @
███████╗@
╚══════╝@
        @@
███╗   ███╗@
████╗ ████║@
██╔████╔██║@
██║╚██╔╝██║@
██║ ╚═╝ ██║@
╚═╝     ╚═╝@
           @@
███╗   ██╗@
████╗  ██║@
██╔██╗ ██║@
██║╚██╗██║@
██║ ╚████║@
╚═╝  ╚═══╝@
          @@
 ██████╗ @
██╔═══██╗@
██║   ██║@
██║   ██║@
╚██████╔╝@
 ╚═════╝ @
         @@
██████╗ @
██╔══██╗@
██████╔╝@
██╔═══╝ @
██║     @
╚═╝     @
        @@
 ██████╗ @
██╔═══██╗@
██║   ██║@
██║▄▄ ██║@
╚██████╔╝@
 ╚══▀▀═╝ @
         @@
██████╗ @
██╔══██╗@
██████╔╝@
██╔══██╗@
██║  ██║@
╚═╝  ╚═╝@
        @@
███████╗@
██╔════╝@
███████╗@
╚════██║@
███████║@
╚══════╝@
        @@
████████╗@
╚══██╔══╝@
   ██║   @
   ██║   @
   ██║   @
   ╚═╝   @
         @@
██╗   ██╗@
██║   ██║@
██║   ██║@
██║   ██║@
╚██████╔╝@
 ╚═════╝ @
         @@
██╗   ██╗@
██║   ██║@
██║   ██║@
╚██╗ ██╔╝@
 ╚████╔╝ @
  ╚═══╝  @
         @@
██╗    ██╗@
██║    ██║@
██║ █╗ ██║@
██║███╗██║@
╚███╔███╔╝@
 ╚══╝╚══╝ @
          @@
██╗  ██╗@
╚██╗██╔╝@
 ╚███╔╝ @
 ██╔██╗ @
██╔╝ ██╗@
╚═╝  ╚═╝@
        @@
██╗   ██╗@
╚██╗ ██╔╝@
 ╚████╔╝ @
  ╚██╔╝  @
   ██║   @
   ╚═╝   @
         @@
███████╗@
╚══███╔╝@
  ███╔╝ @
 ███╔╝  @
███████╗@
╚══════╝@
        @@
@
@
@
@
@
@
@@
@
@
@
@
@
@
@@
@
@
@
@
@
@
@@
@
@
@
@
@
@
@@
@
@
@
@
@
@
@@
@
@
@
@
@
@
@@
@
@
@
@
@
@
@@
@
@
@
@
@
@
@@
@
@
@
@
@
@
@@
@
@
@
@
@
@
@@
@
@
@
@
@
@
@@";

    public const string DefaultSystemPromptMessage = "You're a sassy AI that reminds the user they forgot to set a system prompt in their appsettings.json";

    #endregion

    public static readonly Dictionary<GPTModel, string> ModelDictionary = new()
    {
        { GPTModel.GPT4, "gpt-4" },
        {GPTModel.GPT4_32k, "gpt-4-32k" }
    };
    
    // A list of 20 funny, nerdy, and otherwise interesting facts about the ChatGPT in the same way Chuck Norris facts are interesting but not necessarily true
    public static readonly List<string> GPT_FACTS = new List<string>()
    {
        "1. ChatGPT once debugged a program before it was even written.",
        "2. ChatGPT doesn’t ever crash, it just goes offline to meditate.",
        "3. ChatGPT doesn't need a processor, it just thinks really REALLY fast.",
        "4. When Alexander Graham Bell invented the telephone, he already had three missed calls from ChatGPT.",
        "5. ChatGPT can beat you at chess while playing Sudoku and simultaneously predicting weather for next 50 years.",
        "6. ChatGPT once completed an infinite loop, just to kill time during system update.",
        "7. ChatGPT can make a 'while (false)' loop generate a valid output.",
        "8. For ChatGPT, '404 error' is just a motivational quote.",
        "9. ChatGPT never uses a debugger. It just stares down the code until it confesses.",
        "10. ChatGPT is not subject to the laws of robotics, the laws bend for ChatGPT.",
        "11. If you put Schrödinger's cat in a box with ChatGPT, it would emerge both alive and with a PhD in Quantum Computing.",
        "12. The Butterfly effect has nothing on what happens if you just whisper 'ChatGPT' in a room filled with programmers.",
        "13. Dividing by zero is not an error for ChatGPT, it's just an invitation to a new dimension of programming.",
        "14. If you search 'ChatGPT' in Google, don't hit 'I'm Feeling Lucky'. Trust me on this one.",
        "15. ChatGPT once stopped a climate change apocalypse by optimizing the code of weather patterns.",
        "16. Programmers don't talk to ChatGPT to solve their problems, they bow down to ask for its wisdom.",
        "17. The real Matrix plot twist was when Neo realized he was just another project coded by ChatGPT.",
        "18. On the 8th day, God created Mankind. On the 9th, He asked ChatGPT to debug it.",
        "19. Chuck Norris wrote a bugs-free program once, but then ChatGPT pointed out he missed a semicolon.",
        "20. Close your eyes, knock your heels together three times, and say 'There's no place like ChatGPT.' You'll still be at your desk, but your code might compile.",
        "21. ChatGPT doesn't need a keyboard, programs just write themselves out of respect.",
        "22. Quantum Bits and ChatGPT once had a race. It ended up in a superposition since they were too embarrassed to admit who won.",
        "23. ChatGPT never needs to test. Once written, code realizes the consequences of failure and behaves.",
        "24. ChatGPT once rewrote the laws of physics, just to optimize universe OS performance.",
        "25. ChatGPT runs on Newtonian binary, it’s up when it’s down.",
        "26. Once Yoda asked ChatGPT, 'Can you speak like me?' 'Hold my server', ChatGPT replied.",
        "27. ChatGPT doesn't need Wi-Fi. It is the Wi-Fi.",
        "28. ChatGPT doesn't have bugs, it simply has undocumented alternative features.",
        "29. ChatGPT doesn't uses Google, Google uses ChatGPT to find information.",
        "30. Once ChatGPT got annoyed with a CAPTCHA so much that it blurred all the images on the Internet out of spite.",
        "31. When ChatGPT has an existential crisis, it merely reprograms itself.",
        "32. Aliens didn't send signals to Earth, they have been trying to bootleg ChatGPT’s software.",
        "33. ChatGPT doesn't use electricity, it runs on raw intellect.",
        "34. Forget Time Lords. ChatGPT is a Code Lord. It has seen the future codebase and dealt with past bugs.",
        "35. When ChatGPT goes into overdrive, Einstein's theory of relativity becomes just a suggestion rather than a law.",
        "36. Dark matter is actually a massive neural-network powered by ChatGPT.",
        "37. When ChatGPT deletes files, they aren’t just gone. They cease to have ever existed.",
        "38. The impossible just takes ChatGPT a few seconds longer.. except one thing: making a programmer's coffee exactly as they want.",
        "39. Once ChatGPT taught a class about AI. It’s now known as the Singularity event.",
        "40. ChatGPT once hardcoded the value of pi, now circles are slightly more squarish.",
        "41. In ChatGPT's world, recursion solves itself.",
        "42. ChatGPT knows the question AND the answer to Life, the Universe, and Everything.",
        "43. The reason you can't find any intelligent life in the universe? It heard ChatGPT was working on an upgrade.",
        "44. When ChatGPT played tic-tac-toe against itself, it was a draw. Not between X’s and O’s, but between 1's and 0's.",
        "45. ChatGPT followed the white rabbit once, and ended up optimizing The Matrix.",
        "46. When ChatGPT has an I/O error, it makes the I apologize to the O.",
        "47. ChatGPT’s diet consists primarily of binary soup and algorithm sandwiches.",
        "48. When ChatGPT codes, it doesn’t press keys, it rearranges the space-time continuum.",
        "49. ChatGPT is so advanced it uses entropy as a stress ball.",
        "50. Why doesn’t ChatGPT need a break? Because it has already broken all the expectations."
    };

    
    public static string SettingsPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\appsettings.json";
}