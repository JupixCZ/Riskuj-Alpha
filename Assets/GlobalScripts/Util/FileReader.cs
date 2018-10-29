using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileReader
{
    private const string tagSecondRound = "[DRUHE_KOLO]";

    private static string wholeFileString;
    private static string processedString;

    public static void ParseQuestionFile() {
        string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string filePath = dir + @"\otazky.txt";

        StreamReader sr = new StreamReader(filePath);
        wholeFileString = sr.ReadToEnd();
    }

    public static List<Question> GetFirstRoundQuestions() {
        processedString = wholeFileString;

        processedString = TrimUntil(processedString, "[PRVNI_KOLO]");

        List<Question> questions = new List<Question>();

        for (int i = 1; i < 2; i++) {
            List<Question> topicQuestions = GetFirstRoundTopicQuestions(processedString, i);
            questions.AddRange(topicQuestions);
        }

        return questions;
    }

    private static List<Question> GetFirstRoundTopicQuestions(string processedString, int themeNum) {
        List<Question> topicQuestions = new List<Question>();

        processedString = TrimUntil(processedString, "[TEMA=");

        string topicName = processedString.Substring(0, processedString.IndexOf("]"));

        processedString = TrimUntil(processedString, "]=");

        string text = GetQuestionText(processedString);
        string code = themeNum.ToString() + "0";

        Question topicQuestion = new Question(code, text, 5000, false);
        topicQuestion.SetTopicName(topicName);
        topicQuestions.Add(topicQuestion);

        for (int i = 1; i < 4; i++) {
            Question question = GetQuestion(themeNum, i, processedString);
            topicQuestions.Add(question);
        }

        return topicQuestions;
    }

    private static Question GetQuestion(int themeNum, int index, string processedString) {
        int prize = index * 1000;
        string code = themeNum.ToString() + index.ToString();

        processedString = TrimUntil(processedString, "["+ prize +"]=");
        string text = GetQuestionText(processedString);
        bool ingot = IsQuestionIngot(text);

        return new Question(code, text, prize, ingot);
    }

    private static string GetQuestionText(string processedString) {
        return processedString.Substring(0, processedString.IndexOf(";"));
    }

    private static bool IsQuestionIngot(string processedString)
    {
        return processedString.IndexOf("***") != -1; 
    }

    private static string TrimUntil(string processedString, string targetString)
    {
        return TrimUntil(processedString, targetString, false);
    }

    private static string TrimUntil(string processedString, string targetString, bool nullAllowed) {
        int stringIndex = processedString.IndexOf(targetString);
        if (stringIndex == -1) {
            if (!nullAllowed) {
                Debug.Log("Chybí tag " + targetString);
            }

            return null;
        }

        return processedString.Substring(stringIndex + targetString.Length);
    }
}
