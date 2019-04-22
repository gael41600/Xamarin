using System;
namespace projet.MODELS
{
    public class Commentaire
    {
        public string titre { get; set; }
        public string texte { get; set; }
        public DateTime date { get; set; }
        public Author author { get; set; }

        public Commentaire(string n, string text, DateTime date, Author author)
        {
            titre = n;
            texte = text;
            this.date = date;
            this.author = author;
        }
    }
}
