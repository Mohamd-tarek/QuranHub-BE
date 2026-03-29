namespace QuranHub.Domain.Models;

public class Group
{
    public int GroupId { get; set; }

    public string Name { get; set; }
    public string? QuranHubUserId { get; set; }
    public QuranHubUser QuranHubUser { get; set; }

    public List<Verse> Verses { get; set; } = new List<Verse>();
    public Group() { }
    public Group(string Name, string QuranHubUserId)
    {
        this.Name = Name;
        this.QuranHubUserId = QuranHubUserId;   
    }

    public Group(string Name, string QuranHubUserId, Verse verse)
    {
        this.Name = Name;
        this.QuranHubUserId = QuranHubUserId;
       
        this.Verses.Add(verse);
    }
    public Group(string Name, string QuranHubUserId, List<Verse> verses)
    {
        this.Name = Name;
        this.QuranHubUserId = QuranHubUserId;
        this.Verses.AddRange(verses);
    }

    public void AddVerse( Verse verse)
    {
        this.Verses.Add(verse);
    }

    public void AddVerses( List<Verse> verses)
    {
        this.Verses.AddRange(verses);
    }

    public void RemoveVerse(Verse verse)
    {
        this.Verses.Remove(verse);
    }
}
