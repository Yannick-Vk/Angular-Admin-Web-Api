namespace Angular_Auth.Utils;

public interface IHtmlTag : IHtmlComponent {
    public IHtmlTag Add(IHtmlTag child);
    public IHtmlTag AddClass(string className);
    public IHtmlTag AddClass(string[] classes);
    public bool HasClass(string className);
}