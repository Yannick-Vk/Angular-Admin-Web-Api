namespace Angular_Auth.Utils;

public interface IHtmlTag {
    public IHtmlTag Add(IHtmlTag child);
    public bool HasChildren();
    public IHtmlTag? LastChild();
}