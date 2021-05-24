using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUIRenderer
{
    public class Control
    {
        public Point position;
        public ConsoleColor backgroundColor;
        public ConsoleColor foregroundColor;

        public void Render()
        {
            throw new NotImplementedException();
        }
    }

    #region Controls
    public class Window : Control
    {
        public Size size;
        public string text = "Window";
        public new void Render()
        {
            Point _curpos = Renderer.GetCursorPosition();

            Renderer.SetCursorPosition(this.position);
            Console.ForegroundColor = this.foregroundColor;
            Console.BackgroundColor = this.backgroundColor;

            if (size.height > 5 && size.width > this.text.Length + 5)
            {
                string topLine = "┌ " + this.text + " " + new string('─', Math.Max(1, size.width - (5 + this.text.Length))) + "┐";
                Console.WriteLine(topLine);

                for (int i = 0; i < size.height - 1; i++)
                {
                    Point curpos = Renderer.GetCursorPosition();
                    Renderer.SetCursorPosition(new Point(position.x, curpos.y));
                    if (i == size.height - 2)
                    {
                        Console.WriteLine("└" + new string('─', Math.Max(1, size.width - 3)) + "┘");
                    }
                    else
                    {
                        Console.WriteLine("│" + new string(' ', Math.Max(1, size.width - 3)) + "│");
                    }
                }
            }
            else
            {

            }

            Console.ResetColor();
            Renderer.SetCursorPosition(_curpos);
        }
    }
    public class Label : Control
    {
        public Size size;
        public string text = "Label";
        public new void Render()
        {
            string wrappedText = WordWrap(text, size.width).Replace("\r", "");

            Point _curpos = Renderer.GetCursorPosition();

            Renderer.SetCursorPosition(this.position);
            Console.ForegroundColor = this.foregroundColor;
            Console.BackgroundColor = this.backgroundColor;
            int i = 0;
            foreach (string item in wrappedText.Split("\n"))
            {
                if (i < size.height)
                {
                    Point curpos = Renderer.GetCursorPosition();
                    Renderer.SetCursorPosition(new Point(position.x, curpos.y));
                    Console.WriteLine(item + new string(' ', Math.Max(0, this.size.width - (item.Length + 1))));
                }
                else
                {
                    break;
                }
                i++;
            }
            Console.ResetColor();
            Renderer.SetCursorPosition(_curpos);
        }
        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>The modified text</returns>
        public static string WordWrap(string text, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return text;

            // Parse each line of text
            for (pos = 0; pos < text.Length; pos = next)
            {
                // Find end of line
                int eol = text.IndexOf("\n", pos);
                if (eol == -1)
                    next = eol = text.Length;
                else
                    next = eol + "\n".Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;
                        if (len > width)
                            len = BreakLine(text, pos, width);
                        sb.Append(text, pos, len);
                        sb.Append("\n");

                        // Trim whitespace following break
                        pos += len;
                        while (pos < eol && Char.IsWhiteSpace(text[pos]))
                            pos++;
                    } while (eol > pos);
                }
                else sb.Append("\n"); // Empty line
            }
            return sb.ToString();
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        private static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;

            // If no whitespace found, break at maximum length
            if (i < 0)
                return max;

            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;

            // Return length of text before whitespace
            return i + 1;
        }
    }
    public class Checkmark : Control
    {
        public string text = "Checkmark";
        public bool active = false;
        public bool toggled = false;
        public ConsoleColor activeBackgroundColor;
        public ConsoleColor activeForegroundColor;
        public new void Render()
        {
            Point _curpos = Renderer.GetCursorPosition();

            Renderer.SetCursorPosition(this.position);
            if (active)
            {
                Console.ForegroundColor = this.activeForegroundColor;
                Console.BackgroundColor = this.activeBackgroundColor;
            }
            else
            {
                Console.ForegroundColor = this.foregroundColor;
                Console.BackgroundColor = this.backgroundColor;
            }

            if (toggled)
            {
                Console.WriteLine("✓ " + text);
            }
            else
            {
                Console.WriteLine("- " + text);
            }

            Console.ResetColor();
            Renderer.SetCursorPosition(_curpos);
        }
    }
    public class Button : Control
    {
        public string text = "Button";
        public bool active = false;
        public ConsoleColor activeBackgroundColor;
        public ConsoleColor activeForegroundColor;
        public new void Render()
        {
            Point _curpos = Renderer.GetCursorPosition();


            Renderer.SetCursorPosition(this.position);
            if (active)
            {
                Console.ForegroundColor = this.activeForegroundColor;
                Console.BackgroundColor = this.activeBackgroundColor;
            }
            else
            {
                Console.ForegroundColor = this.foregroundColor;
                Console.BackgroundColor = this.backgroundColor;
            }

            Console.WriteLine("[ " + text + " ]");

            Console.ResetColor();
            Renderer.SetCursorPosition(_curpos);
        }
    }
    public class ListBox : Control
    {
        public Size size;
        public int selectedIndex;
        public ConsoleColor activeBackgroundColor;
        public ConsoleColor activeForegroundColor;
        public List<string> items = new List<string>();
        public new void Render()
        {
            Point _curpos = Renderer.GetCursorPosition();

            Renderer.SetCursorPosition(this.position);
            Console.ForegroundColor = this.foregroundColor;
            Console.BackgroundColor = this.backgroundColor;

            int i = 0;
            foreach (string item in items)
            {
                Point curpos = Renderer.GetCursorPosition();
                Renderer.SetCursorPosition(new Point(position.x, curpos.y));
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = this.activeForegroundColor;
                    Console.BackgroundColor = this.activeBackgroundColor;
                    Console.WriteLine(item + new string(' ', Math.Max(0, this.size.width - item.Length)));
                    Console.ForegroundColor = this.foregroundColor;
                    Console.BackgroundColor = this.backgroundColor;
                }
                else
                {
                    Console.WriteLine(item + new string(' ', Math.Max(0, this.size.width - item.Length)));
                }
                i++;
            }
            for (int x = 0; x < this.size.height - this.items.Count; x++)
            {
                Point curpos = Renderer.GetCursorPosition();
                Renderer.SetCursorPosition(new Point(position.x, curpos.y));
                Console.WriteLine(new string(' ', Math.Max(0, this.size.width)));
            }

            Console.ResetColor();
            Renderer.SetCursorPosition(_curpos);
        }
    }
    public class ProgressBar : Control
    {

        public Size size;
        public ConsoleColor activeBackgroundColor;
        public ConsoleColor activeForegroundColor;
        public int value;
        public new void Render()
        {
            Point _curpos = Renderer.GetCursorPosition();

            Renderer.SetCursorPosition(this.position);
            Console.ForegroundColor = this.activeForegroundColor;
            Console.BackgroundColor = this.activeBackgroundColor;

            Console.Write(new string(' ', Math.Max(0, Math.Min(this.size.width, value))));

            Console.ForegroundColor = this.foregroundColor;
            Console.BackgroundColor = this.backgroundColor;

            Console.WriteLine(new string(' ', Math.Max(0, this.size.width - value)));

            Console.ResetColor();
            Renderer.SetCursorPosition(_curpos);
        }
    }
    public class TextBox : Control
    {
        public string text;

    }
    #endregion
    public class Point
    {
        public int x;
        public int y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public class Size
    {
        public int width;
        public int height;
        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
    public class Renderer
    {
        public static void SetCursorPosition(Point point)
        {
            Console.SetCursorPosition(point.x, point.y);
        }
        public static Point GetCursorPosition()
        {
            return new Point(Console.CursorLeft, Console.CursorTop);
        }
        public static void SetText(string text)
        {

        }
    }
}