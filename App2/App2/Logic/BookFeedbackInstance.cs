using System;
using System.Text.Json.Serialization;

namespace App2.Logic;

/// <summary>
/// 책에 읽은 후 감상문 정보에 대한 인스턴스. 
/// </summary>
public class BookFeedbackInstance
{
    [JsonConstructor]
    public BookFeedbackInstance(string title,
        string isbn,
        string description,
        DateTime startTime,
        DateTime endTime,
        int readPage = 0,
        float rating = 0f)
    {
        Title = title;
        Isbn = isbn;
        Description = description;
        IconPath = MainWindow.books[isbn].BookIconPath;
        ReadPage = readPage;
        Rating = rating;
        StartTime = startTime;
        EndTime = endTime;
    }

    public static BookFeedbackInstance[] TestItems =
    {
        new BookFeedbackInstance("Hello World!",
            "9788936434120", "This is Simple Description", DateTime.Now, DateTime.Now),

        new BookFeedbackInstance("Hello World!",
            "9788936434120", "This is Simple Description", DateTime.Now, DateTime.Now),

        new BookFeedbackInstance("Hello World!",
            "9788936434120", "This is Simple Description", DateTime.Now, DateTime.Now),
        new BookFeedbackInstance("Hello World!",
            "9788936434120", "This is Simple Description", DateTime.Now, DateTime.Now),
        new BookFeedbackInstance("Hello World!",
            "9788936434120", "This is Simple Description", DateTime.Now, DateTime.Now),
        new BookFeedbackInstance("Hello World!",
            "9788936434120", "This is Simple Description", DateTime.Now, DateTime.Now),
    };
    [JsonInclude]
    public string Title { get; set; }
    [JsonInclude]
    public string Isbn { get; set; }
    [JsonInclude]
    public string Description { get; set; }
    public string IconPath { get; set; }
    [JsonInclude]
    public int ReadPage { get; set; }
    [JsonInclude]
    public float Rating { get; set; }
    [JsonInclude]
    public DateTime StartTime { get; set; }
    [JsonInclude]
    public DateTime EndTime { get; set; }

    public TimeSpan GetStartTimeSpan()
    {
        return new TimeSpan(StartTime.Hour, StartTime.Minute, StartTime.Second);
    }

    public TimeSpan GetEndTimeSpan()
    {
        return new TimeSpan(EndTime.Hour, EndTime.Minute, EndTime.Second);
    }
}