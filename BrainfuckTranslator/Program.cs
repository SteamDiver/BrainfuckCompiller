using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BrainfuckTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StreamReader fileStream = new StreamReader("test.txt"))
            {
                AntlrInputStream input = new AntlrInputStream(fileStream);
                pascalLexer lexer = new pascalLexer(input);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                pascalParser parser = new pascalParser(tokens);

                var result = parser.program();

                List<String> ruleNamesList = new List<string>(parser.RuleNames);
                String prettyTree = TreeUtils.toPrettyTree(result, ruleNamesList);
                Console.WriteLine(prettyTree);
                Console.ReadKey();
            }
        }

        //private List<IParseTree> GetChildren(ParserRuleContext context)
        //{
        //    var tree = new List<IParseTree>();
        //    foreach (var tr in context.children)
        //    {
        //        tree.Add(GetChildren(tr.));
        //    }
        //    return tree;
        //}
    }
    public class TreeUtils
    {

        /** Platform dependent end-of-line marker */
        public static String Eol = "\r\n";
        /** The literal indent char(s) used for pretty-printing */
        public static String Indents = " ";
        private static int level;

        private TreeUtils() { }

        /**
         * Pretty print out a whole tree. {@link #getNodeText} is used on the node payloads to get the text
         * for the nodes. (Derived from Trees.toStringTree(....))
         */
        public static String toPrettyTree(ITree t, List<String> ruleNames)
        {
            level = 0;
            return process(t, ruleNames, 0).Replace("(?m)^\\s+$", "").Replace("\\r?\\n\\r?\\n", Eol);
        }

        private static String process(ITree t, List<String> ruleNames, int baseLevel)
        {
            if (t.ChildCount == 0) return Utils.EscapeWhitespace(Trees.GetNodeText(t, ruleNames), false);
            StringBuilder sb = new StringBuilder();
            sb.Append(lead(level, baseLevel));
            level++;
            String s = Utils.EscapeWhitespace(Trees.GetNodeText(t, ruleNames), false);
            sb.Append(s + ' ');
            for (int i = 0; i < t.ChildCount; i++)
            {
                var node = process(t.GetChild(i), ruleNames, level);
                sb.Append(node);
            }
            level--;
           
            sb.Append(lead(level, level));
            return sb.ToString();
        }

        private static String lead(int level, int baseLevel)
        {
            StringBuilder sb = new StringBuilder();
            if (level > 0)
            {
                sb.Append(Eol);
                for (int cnt = 0; cnt < baseLevel; cnt++)
                {
                    sb.Append(Indents);
                }
                sb.Append("|-");
            }
            return sb.ToString();
        }
    }

    public class ErrorListener : BaseErrorListener
    {
        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            Console.WriteLine("{0}: line {1}/column {2} {3}", e, line, charPositionInLine, msg);
        }
    }
}
