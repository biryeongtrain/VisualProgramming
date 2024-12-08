using System;
using System.IO;
using System.Text.Json.Serialization;
using App2.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using BooksControl = App2.Views.Controls.BooksControl;

namespace App2.Logic;


public class BookData : ComponentFactory<BooksControl>
{
    [JsonConstructor]
    public BookData(double rating, string bookIconPath, string bookTitle, string isbn, string bookDescription, int currentPage, int totalPages)
    {
        _rating = rating;
        BookIconPath = bookIconPath;
        BookTitle = bookTitle;
        BookDescription = bookDescription;
        CurrentPage = currentPage;
        TotalPages = totalPages;
        Isbn = isbn;
    }

    [JsonInclude]
    public String BookIconPath
    {
        get;
    }

    [JsonInclude]
    public String BookTitle
    {
        get;
    }

    [JsonInclude]
    public String BookDescription
    {
        get;
    }

    [JsonInclude]
    public string Isbn
    {
        get;
    }

    private double _rating;
    [JsonInclude]
    public double Rating
    {
        get => this._rating;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            this._rating = value;
        }
    }

    [JsonInclude]
    public int CurrentPage
    {
        get;
        set;
    }

    [JsonInclude]
    public int TotalPages
    {
        get;
    }


    /**
     * BookControl 컴포넌트를 생성
     */

    public override BooksControl CreateComponent()
    {
        var image = new Image()
        {
            Source = new BitmapImage(new Uri(this.BookIconPath))
        };
        return new BooksControl()
        {
            Title = this.BookTitle,
            Description = this.BookDescription,
            Rating = this.Rating.ToString(),
            Source = image,
            ReadPercent = this.TotalPages == 0 ? 100 : (float)this.CurrentPage / this.TotalPages,
            Isbn = this.Isbn
        };
    }
}