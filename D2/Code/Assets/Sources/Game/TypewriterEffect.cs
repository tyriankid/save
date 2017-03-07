using UnityEngine;

/// <summary>
/// Trivial script that fills the label's contents gradually, as if someone was typing.
/// </summary>

[RequireComponent(typeof(UILabel))]
public class TypewriterEffect : MonoBehaviour
{
	public int charsPerSecond = 40;

	private UILabel mLabel;
    private string mText;
    private int mOffset = 0;
    private float mNextChar = 0f;

    public void UpdateText(UILabel label)
    {
        mOffset = 0;
        mNextChar = 0f;
        mText = label.text;
        mLabel = label;
        mLabel.text = "";

        this.enabled = true;
    }

	void Update ()
	{
        //if (mLabel == null)
        //{
        //    mLabel = GetComponent<UILabel>();
        //    mLabel.supportEncoding = false;
        //    mLabel.symbolStyle = UIFont.SymbolStyle.None;
        //    mLabel.bitmapFont.WrapText(mLabel.text, mLabel.width, out mText, mLabel.width, mLabel.height, mLabel.maxLineCount, false, UIFont.SymbolStyle.None);
        //}

        if (mOffset < mText.Length)
        {
            if (mNextChar <= Time.time)
            {
                charsPerSecond = Mathf.Max(1, charsPerSecond);

                // Periods and end-of-line characters should pause for a longer time.
                float delay = 1f / charsPerSecond;
                char c = mText[mOffset];
                if (c == '.' || c == '\n' || c == '!' || c == '?') delay *= 4f;

                mNextChar = Time.time + delay;
                mLabel.text = mText.Substring(0, ++mOffset);
            }
        }
        else
        {
            this.enabled = false;
        }
	}
}
