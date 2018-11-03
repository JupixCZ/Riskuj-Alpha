using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileReader
{
    private const string tagSecondRound = "[DRUHE_KOLO]";
    private static List<string> secondRoundQuestionCodes= new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "12", "14", "16", "18", "20", "22", "24",
    "26", "28", "30", "32", "34", "36", "37", "40", "43", "46", "49", "52", "55", "58", "61", "64", "67", "70"}; 

    private static string wholeFileString;
    private static string processedString;

    public static void ParseQuestionFile() {
        string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string filePath = dir + @"\otazky.txt";

        StreamReader sr = new StreamReader(filePath);
        wholeFileString = sr.ReadToEnd();
    }

    public static List<Question> GetFirstRoundQuestions() {
        int topicCount = GlobalHandler.IsDebugMode() ? 2 : 6;
        processedString = wholeFileString;

        processedString = TrimUntil(processedString, "[PRVNI_KOLO]");

        List<Question> questions = new List<Question>();

        for (int i = 1; i < topicCount; i++) {
            List<Question> topicQuestions = GetFirstRoundTopicQuestions(i);
            questions.AddRange(topicQuestions);
        }

        return questions;
    }

    private static List<Question> GetFirstRoundTopicQuestions(int themeNum) {
        int questionCount = GlobalHandler.IsDebugMode() ? 3 : 6;

        List<Question> topicQuestions = new List<Question>();

        processedString = TrimUntil(processedString, "[TEMA=");

        string topicName = processedString.Substring(0, processedString.IndexOf("]"));

        processedString = TrimUntil(processedString, "]=");

        string text = GetQuestionText(processedString);
        string code = themeNum.ToString() + "0";

        Question topicQuestion = new Question(code, text, 5000, false);
        topicQuestion.SetTopicName(topicName);
        topicQuestions.Add(topicQuestion);

        for (int i = 1; i < questionCount; i++) {
            Question question = GetQuestion(themeNum, i, processedString);
            topicQuestions.Add(question);
        }

        return topicQuestions;
    }

    public static List<Question> GetSecondRoundQuestions() {
        processedString = wholeFileString;

        processedString = TrimUntil(processedString, "[DRUHE_KOLO]");

        List<Question> questions = new List<Question>();

        foreach (string code in secondRoundQuestionCodes)
        {
            Question question = GetQuestion(code, processedString);
            questions.Add(question);
        }

        return questions;
    }

    private static Question GetQuestion(int themeNum, int index, string processedString) {
        int prize = index * 1000;
        string code = themeNum.ToString() + index.ToString();

        processedString = TrimUntil(processedString, "["+ prize +"]=");
        string text = GetQuestionText(processedString);
        bool ingot = IsQuestionIngot(text);

        return new Question(code, text, prize, ingot);
    }

    private static Question GetQuestion(string code, string processedString)
    {
        int prize = Convert.ToInt32(code) * 1000;

        processedString = TrimUntil(processedString, "[" + prize + "]=");
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
