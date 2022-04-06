using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace FreeCellMetrics.Classes
{
    //Tag="00S13"
    public class TagReader
    {
        internal static void Populate(string startTag, Grid grid)
        {
            //set new tagS
            //validate
            //remove items

            List<Button> buttons = new List<Button>();
            foreach (Button item in grid.Children)
            {
                Button clickedBtn;

                if (item.Content is Image)
                    clickedBtn = (Button)((Image)item.Content).Parent;
                else
                    clickedBtn = (Button)item;

                 buttons.Add(clickedBtn);
            }

           var startButton = buttons.GroupBy(x => x.Content.ToString())
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

           if (startButton.Count > 0)
           {
               var removeButton = buttons.Where(b => b.Content.ToString() == startButton[0].ToString()).ToArray();
               for (int i = 0; i < removeButton.Count(); i++ )
                   buttons.Remove(((Button)removeButton[i]));
           }
           
            grid.Children.RemoveRange(0, grid.Children.Count);

            foreach (var item in buttons)
            {
                Button button0 = new Button()
                {
                    
                    Content = item.Content,
                    Tag = (TagReader.GetRow(item.Tag.ToString())
                            + TagReader.GetCol(item.Tag.ToString()
                            + TagReader.GetColor(item.Tag.ToString())
                            + TagReader.GetValue(item.Tag.ToString())))
                };

                Grid.SetRow(button0, TagReader.GetRow(item.Tag.ToString()));
                Grid.SetColumn(button0, TagReader.GetCol(item.Tag.ToString()));
                
                grid.Children.Add(item);
            }
        }

        public static Card Read(string tag)
        {
            var res = new Card();
            res.Position.row = GetRow(tag);
            res.Position.col = GetCol(tag);

            res.Color = GetColor(tag);

            res.isRed = parseColorRed(res);

            res.Value = int.Parse(GetValue(tag));
            //todo TAG?
            return res;
        }

        public static bool parseColorRed(Card res)
        {
            if ((res.Color == "S" || res.Color == "C"))
                return false;
            else
                return true;
        }

        internal static Button FindSelected(string tag, UIElementCollection uIElementCollection)
        {
            Button res = new Button();

            int row = TagReader.GetRow(tag);
            int col = TagReader.GetCol(tag);
            string color = TagReader.GetColor(tag);
            string val = TagReader.GetValue(tag);

            if (row == 0)
            {
                Card selC = new Card();
                selC.Color = color;
                selC.Value = int.Parse(val);
                selC.isRed = parseColorRed(selC);

                foreach (Button item in uIElementCollection)
                {
                    Card itemC = new Card();

                    itemC.Value = int.Parse(GetValue(item.Tag.ToString()));
                    itemC.Color = GetColor(item.Tag.ToString());
                    itemC.isRed = parseColorRed(itemC);

                    itemC.Position.col = GetCol(item.Tag.ToString());
                    itemC.Position.row = GetRow(item.Tag.ToString());
                    //
                    //set top cards
                    List<Card> cards = PopulateCards(uIElementCollection);
                    for (int k = 1; k <= 8; k++)
                    {
                        Card topCard = new Card();
                        topCard.Position.row = 1;

                        foreach (Card c in cards)
                        {
                            if (c.Position.col == k)
                            {
                                if (c.Position.row > topCard.Position.row)
                                {
                                    topCard = c;
                                }
                            }
                        }

                        //todo reduce # of collections in this function
                        if (itemC.Value == topCard.Value && itemC.Color == topCard.Color)
                            itemC.isTop = true;

                        topCard.isTop = true;
                    }

                    //find selected
                    if (itemC.isRed && !selC.isRed || selC.isRed && !itemC.isRed)
                    {
                        if (itemC.Value == selC.Value + 1 && itemC.isTop)
                            return item;
                    }
                }
            }

            return null;
        }

        internal static List<Button> FindTail(string t, System.Windows.Controls.UIElementCollection uIElementCollection)
        {
            List<Button> res = new List<Button>();

            int row = TagReader.GetRow(t);
            int col = TagReader.GetCol(t);

            foreach (Button item in uIElementCollection)
            {
                int btnRow = TagReader.GetRow(item.Tag.ToString());
                int btnCol = TagReader.GetCol(item.Tag.ToString());

                if (btnRow > row && btnCol == col)
                {
                    res.Add(item);
                } 
            }
            //
            List<Button> lsit = res.OrderByDescending(c => GetValue(c.Tag.ToString())).ToList();
            Dictionary<string, Button> pair = new Dictionary<string, Button>();
            pair.Add("1", null);
            pair.Add("2", null);
            pair.Add("3", null);
            pair.Add("4", null);
            pair.Add("5", null);
            pair.Add("6", null);
            pair.Add("7", null);
            pair.Add("8", null);
            pair.Add("9", null);
            pair.Add("10", null);
            pair.Add("11", null);
            pair.Add("12", null);
            pair.Add("13", null);
            pair.Add("14", null);

            foreach (Button item in lsit)
            {
                if (GetValue(item.Tag.ToString()) == 1.ToString())
                    pair["1"] = item;
                if (GetValue(item.Tag.ToString()) == 2.ToString())
                    pair["2"] = item;
                if (GetValue(item.Tag.ToString()) == 3.ToString())
                    pair["3"] = item;
                if (GetValue(item.Tag.ToString()) == 4.ToString())
                    pair["4"] = item;
                if (GetValue(item.Tag.ToString()) == 5.ToString())
                    pair["5"] = item;
                if (GetValue(item.Tag.ToString()) == 6.ToString())
                    pair["6"] = item;
                if (GetValue(item.Tag.ToString()) == 7.ToString())
                    pair["7"] = item;
                if (GetValue(item.Tag.ToString()) == 8.ToString())
                    pair["8"] = item;
                if (GetValue(item.Tag.ToString()) == 9.ToString())
                    pair["9"] = item;
                if (GetValue(item.Tag.ToString()) == 10.ToString())
                    pair["10"] = item;
                if (GetValue(item.Tag.ToString()) == 11.ToString())
                    pair["11"] = item;
                if (GetValue(item.Tag.ToString()) == 12.ToString())
                    pair["12"] = item;
                if (GetValue(item.Tag.ToString()) == 13.ToString())
                    pair["13"] = item;
                if (GetValue(item.Tag.ToString()) == 14.ToString())
                    pair["14"] = item;
            }

            List<Button> resList = new List<Button>();

            for (int i = 14; i > 0; i--)
            {
                Button item = pair[i.ToString()];
                
                if (item == null) continue;

                resList.Add(item);
            }

            //
            return resList;
        }

        internal static int GetCol(string t)
        {
            int res = 0;
            try
            {
                int row = GetRow(t);
                if (row > 9)
                {
                    res = int.Parse(t.Substring(2, 1));
                }
                else
                {
                    res = int.Parse(t.Substring(1, 1));
                }
            }
            catch { }

            return res;
        }

        internal static int GetRow(string t)
        {
            int row = 0;
            try
            {
                string lead = string.Empty;
                int sep = t.IndexOfAny(new char[] { 'D', 'S', 'H', 'C' });

                for (int i = 0; i < sep; i++)
                {
                    lead += t[i];
                }

                lead = lead.Substring(0, lead.Length - 1);

                row = int.Parse(lead);
            }
            catch { }

            return row;
        }

        internal static string GetColor(string t)
        {
            try
            {
                int row = GetRow(t);
                if (row > 9)
                {
                    t = t.Substring(3, 1);
                }
                else
                {
                    t = t.Substring(2, 1);
                }
            }
            catch { }

            return t;
        }

        internal static string GetValue(string t)
        {
            string resStr = string.Empty;

            try
            {
                int row = GetRow(t);
                if (row > 9)
                {
                    string lead = t.Substring(4, 1);
                    if (lead == "1" && lead.Length == 2)
                    {
                        row = int.Parse(t.Substring(0, 2));
                    }
                    else
                    {
                        row = int.Parse(t.Substring(0, 1));
                    }

                    resStr = t.Substring(4, t.Length - 4);
                }
                else
                {
                    resStr = t.Substring(3, t.Length - 3);
                }
            }
            catch { }

            return resStr;
            //
        }

        internal static string GenTag(string ot, int r, int c)
        {
            StringBuilder res = new StringBuilder(r.ToString() + c.ToString() +
                    GetColor(ot) + GetValue(ot));
            return res.ToString();
        }

        internal static List<Card> PopulateCards(UIElementCollection uIElementCollection)
        {
            List<Card> cards = new List<Card>();

            foreach (Button item in uIElementCollection)
            {
                Card card = new Card();

                card.Value = int.Parse(GetValue(item.Tag.ToString()));
                card.Color = GetColor(item.Tag.ToString());
                card.isRed = parseColorRed(card);

                card.Position.col = GetCol(item.Tag.ToString());
                card.Position.row = GetRow(item.Tag.ToString());

                cards.Add(card);
            }

            //set top cards
            for (int k = 1; k <= 8; k++)
            {
                Card topCard = new Card();
                topCard.Position.row = 1; 

                foreach (Card c in cards)
                {
                    if (c.Position.col == k)
                    {
                        if (c.Position.row > topCard.Position.row)
                        {
                            topCard = c;
                        }
                    }
                }

                topCard.isTop = true;
            }

            //set top mius 1 cards
            for (int k = 1; k <= 8; k++)
            {
                Card topMinusOneCard = new Card();
                topMinusOneCard.Position.row = 1;

                foreach (Card c in cards)
                {
                    if (c.Position.col == k)
                    {
                        if (c.isTop)
                        {
                            break;
                        }

                        if (c.Position.row > topMinusOneCard.Position.row)
                        {
                            topMinusOneCard = c;
                        }
                    }
                }

                topMinusOneCard.isTopMinusOne = true;
            }

            return cards;
        }       
    }
}
