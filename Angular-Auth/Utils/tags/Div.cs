namespace Angular_Auth.Utils.tags;

public class Div(IHtmlComponent content) : HtmlTag("div", content) {
    public Div() : this(new Text("")) { }
}