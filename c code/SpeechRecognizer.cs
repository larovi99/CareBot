using System;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;


public class SpeechRecognizer : MonoBehaviour
{
    public string[] m_Keywords;
    private KeywordRecognizer m_Recognizer;
    public string word = "";
    void Start()
    {
        Debug.Log("Waiting for instructions");
        m_Keywords = new string[] { "move", "stop","take bottle", "take apple", "bottle", "apple"};
        m_Recognizer = new KeywordRecognizer(m_Keywords, ConfidenceLevel.Low);
        m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
        m_Recognizer.Start();
        //Debug.Log("Started");
    }
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Phrase received");
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
        builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
        builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
        Debug.Log(builder.ToString());
        word= args.text;
    }
    
}