using BookStore.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Models
{
    public class BooksData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BookStoreContext(serviceProvider.GetRequiredService<DbContextOptions<BookStoreContext>>()))
            {
                if (context.Books.Any())   // Check if database contains any books
                {
                    return;     // Database contains books already
                }

                context.Books.AddRange(
                    new Book
                    {
                        Title = "The Question of Palestine",
                        Language = "English",
                        ISBN = "9780394745276",
                        PublicationDate = DateTime.Parse("1979-1-1"),
                        Price = 199,
                        Author = "Edward W. Said",
                        CoverImageUrl = "/images/question_of_palestine.jpg",
                        Description = "The Question of Palestine, written by Edward W. Said, explores the historical, political, and cultural dimensions of the Palestinian struggle for self-determination. The book examines the Palestinian experience, Zionism, media representation, and international diplomacy in the context of the Middle Eastern conflict.",
                        Category = "Non-Fiction",
                        NumberOfPages = 288
                    },

                    new Book
                    {
                        Title = "Palestine: A Socialist Introduction",
                        Language = "English",
                        ISBN = "9781642593862",
                        PublicationDate = DateTime.Parse("2020-12-22"),
                        Price = 89,
                        Author = "Sumaya Awad, Brian Bean",
                        CoverImageUrl = "/images/palestine_socialist_introduction.png",
                        Description = "Palestine: A Socialist Introduction provides an analysis of the Palestinian struggle from a socialist perspective. Edited by Sumaya Awad and Brian Bean, this book brings together essays from activists and scholars that challenge the mainstream narratives about Palestine, offering insights into the intersection of anti-imperialism, anti-racism, and socialist politics.",
                        Category = "Politics",
                        NumberOfPages = 260

                    },

                    new Book
                    {
                        Title = "Palestine Speaks: Narratives of Life Under Occupation",
                        Language = "English",
                        ISBN = "9781566567087",
                        PublicationDate = new DateTime(2012, 9, 4),
                        Price = 18.95m,
                        Author = "Anna Baltzer",
                        CoverImageUrl = "/images/palestine_speaks.jpg",
                        Description = "Palestine Speaks: Narratives of Life Under Occupation presents personal stories from Palestinians living under Israeli occupation. Edited by Anna Baltzer, the book offers a collection of powerful testimonies from individuals across the West Bank and Gaza Strip, shedding light on their daily struggles, hopes, and resilience. It provides readers with an intimate understanding of the human impact of the ongoing conflict and occupation.",
                        Category = "Non-fiction, Middle Eastern Studies",
                        NumberOfPages = 240

                    },

                    new Book
                    {
                        Title = "A Child in Palestine: The Cartoons of Naji al-Ali",
                        Language = "English",
                        ISBN = "9780863565384",
                        PublicationDate = new DateTime(2010, 4, 15),
                        Price = 22.95m,
                        Author = "Naji al-Ali",
                        CoverImageUrl = "/images/a_child_in_palestine.jpg",
                        Description = "A Child in Palestine: The Cartoons of Naji al-Ali showcases the powerful and poignant cartoons of Naji al-Ali, a renowned Palestinian cartoonist. This collection reflects the struggles and aspirations of Palestinians through sharp and insightful political commentary. Al-Ali's work provides a unique perspective on the Palestinian experience and the broader conflict, capturing the resilience and spirit of the people.",
                        Category = "Art, Middle Eastern Studies",
                        NumberOfPages = 160
                    },

                    new Book
                    {
                        Title = "Gaza: A History",
                        Language = "English",
                        ISBN = "9780300217826",
                        PublicationDate = new DateTime(2014, 9, 30),
                        Price = 24.95m,
                        Author = "Jean-Pierre Filiu",
                        CoverImageUrl = "/images/gaza_a_history.jpg",
                        Description = "Gaza: A History by Jean-Pierre Filiu provides an in-depth historical account of the Gaza Strip, tracing its significance from ancient times to the present. This comprehensive work explores the region's complex history, including its strategic importance and the impact of various political, religious, and social dynamics. Filiu offers insights into the enduring conflicts and challenges faced by Gaza and its people.",
                        Category = "History, Middle Eastern Studies",
                        NumberOfPages = 368
                    },

                    new Book
                    {
                      
                        Title = "Our Way to Fight: Peace-Work Under Siege in Israel-Palestine",
                        Language = "English",
                        ISBN = "9780863565254",
                        PublicationDate = new DateTime(2008, 1, 1),
                        Price = 21.95m,
                        Author = "Yehuda Shaul",
                        CoverImageUrl = "/images/our_way_to_fight.jpg",
                        Description = "Our Way to Fight: Peace-Work Under Siege in Israel-Palestine by Yehuda Shaul offers a compelling account of the challenges faced by peace activists working in Israel-Palestine amidst ongoing conflict. The book explores the personal and political struggles of those dedicated to advocating for peace, providing a unique perspective on their efforts and the obstacles they encounter.",
                        Category = "Politics, Middle Eastern Studies",
                        NumberOfPages = 280

                    }
                );

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {Console.WriteLine("An error occurred: " + ex.Message);
                    Console.WriteLine("Inner Exception: " + ex.InnerException?.Message);
                }
            }
        }
    }
}
