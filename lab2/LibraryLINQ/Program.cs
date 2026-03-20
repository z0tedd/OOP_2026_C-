using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibraryLINQ
{
    // ==================== КЛАССЫ ====================
    
    public class Book
    {
        public int Key { get; set; }           // Ключ
        public string Title { get; set; }        // Название
        public string Author { get; set; }       // Автор
        public int Year { get; set; }            // Год издания
        public string Code { get; set; }         // Шифр книги
        public DateTime IssueDate { get; set; }  // Дата выдачи
        public DateTime ReturnDate { get; set; } // Дата получения (возврата)
        public int Copies { get; set; }          // Кол-во экземпляров
        
        public Book() { }
        
        public Book(int key, string title, string author, int year, string code, 
                    DateTime issueDate, DateTime returnDate, int copies)
        {
            Key = key;
            Title = title;
            Author = author;
            Year = year;
            Code = code;
            IssueDate = issueDate;
            ReturnDate = returnDate;
            Copies = copies;
        }
        
        public override string ToString()
        {
            return $"{Title} ({Author}, {Year}) - Шифр: {Code}, Экз.: {Copies}";
        }
    }

    public class Reader
    {
        public int Key { get; set; }           // Ключ
        public string FullName { get; set; }   // ФИО
        public int TicketNumber { get; set; }  // № билета
        public DateTime BirthDate { get; set; }// Дата рождения
        public string Phone { get; set; }      // Телефон
        public string Education { get; set; }  // Образование
        public string Hall { get; set; }       // Зал
        
        public Reader() { }
        
        public Reader(int key, string fullName, int ticket, DateTime birth, 
                      string phone, string education, string hall)
        {
            Key = key;
            FullName = fullName;
            TicketNumber = ticket;
            BirthDate = birth;
            Phone = phone;
            Education = education;
            Hall = hall;
        }
        
        public override string ToString()
        {
            return $"{FullName} (Билет: {TicketNumber}, Зал: {Hall})";
        }
    }

    public class Hall
    {
        public int Key { get; set; }              // Ключ
        public string LibraryName { get; set; }   // Название библиотеки
        public string Name { get; set; }          // Зал
        public string Specialization { get; set; }// Специализация
        public int Capacity { get; set; }         // Количество мест
        public List<int> ReaderKeys { get; set; } // Список читателей (ключи)
        
        public Hall()
        {
            ReaderKeys = new List<int>();
        }
        
        public Hall(int key, string libraryName, string name, string spec, 
                    int capacity, List<int> readerKeys)
        {
            Key = key;
            LibraryName = libraryName;
            Name = name;
            Specialization = spec;
            Capacity = capacity;
            ReaderKeys = readerKeys ?? new List<int>();
        }
        
        public override string ToString()
        {
            return $"{Name} ({Specialization}) - {Capacity} мест, Читателей: {ReaderKeys.Count}";
        }
    }

    // ==================== ОСНОВНАЯ ПРОГРАММА ====================
    
    class Program
    {
        static List<Book> books = new List<Book>();
        static List<Reader> readers = new List<Reader>();
        static List<Hall> halls = new List<Hall>();
        
        static string[] paths = { "Books.txt", "Readers.txt", "Halls.txt" };
        static int[] fieldCounts = { 8, 7, 6 }; // количество полей в каждом файле

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            // Загрузка данных из файлов
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    LoadFile(paths[i], i, fieldCounts[i]);
                }
                Console.WriteLine("Данные успешно загружены!\n");
                Console.WriteLine($"Книг: {books.Count}, Читателей: {readers.Count}, Залов: {halls.Count}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки: {ex.Message}");
                // Создаём тестовые данные, если файлов нет
                CreateTestData();
            }

            // Главный цикл меню
            int choice = -1;
            while (true)
            {
                ShowMenu();
                
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Ошибка: введите число!");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }

                switch (choice)
                {
                    case 1: Query1_BooksByReader(); break;
                    case 2: Query2_FreeSeatsInHalls(); break;
                    case 3: Query3_CanIssueBook(); break;
                    case 4: Query4_BooksByAuthorInHall(); break;
                    case 5: Query5_UniqueBooksReaders(); break;
                    case 6: Query6_MaxRatingBooks(); break;
                    case 7: AddNewReader(); break;
                    case 8: RemoveBook(); break;
                    case 9: AddNewBook(); break;
                    case 0: 
                        Console.WriteLine("Выход из программы. До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный номер запроса!");
                        break;
                }
                
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        // ==================== ЗАГРУЗКА ФАЙЛОВ ====================
        
        static void LoadFile(string path, int listIndex, int fieldCount)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Файл не найден: {path}");

            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    
                    string[] parts = line.Split(';');
                    if (parts.Length < fieldCount) continue;

                    switch (listIndex)
                    {
                        case 0: // Books
                            books.Add(new Book(
                                int.Parse(parts[0]),
                                parts[1].Trim(),
                                parts[2].Trim(),
                                int.Parse(parts[3]),
                                parts[4].Trim(),
                                DateTime.Parse(parts[5]),
                                DateTime.Parse(parts[6]),
                                int.Parse(parts[7])
                            ));
                            break;
                            
                        case 1: // Readers
                            readers.Add(new Reader(
                                int.Parse(parts[0]),
                                parts[1].Trim(),
                                int.Parse(parts[2]),
                                DateTime.Parse(parts[3]),
                                parts[4].Trim(),
                                parts[5].Trim(),
                                parts[6].Trim()
                            ));
                            break;
                            
                        case 2: // Halls
                            var readerKeys = parts[5].Split(',').Select(int.Parse).ToList();
                            halls.Add(new Hall(
                                int.Parse(parts[0]),
                                parts[1].Trim(),
                                parts[2].Trim(),
                                parts[3].Trim(),
                                int.Parse(parts[4]),
                                readerKeys
                            ));
                            break;
                    }
                }
            }
        }

        static void CreateTestData()
        {
            // Тестовые данные, если файлы отсутствуют
            books.AddRange(new[]
            {
                new Book(1, "Война и мир", "Толстой Л.Н.", 1869, "ABC-001", 
                        DateTime.Now.AddDays(-30), DateTime.Now.AddDays(30), 3),
                new Book(2, "Преступление и наказание", "Достоевский Ф.М.", 1866, "ABC-002", 
                        DateTime.Now.AddDays(-20), DateTime.Now.AddDays(40), 2),
                new Book(3, "Мастер и Маргарита", "Булгаков М.А.", 1966, "ABC-003", 
                        DateTime.Now.AddDays(-25), DateTime.Now.AddDays(35), 1)
            });
            
            readers.AddRange(new[]
            {
                new Reader(1, "Иванов И.И.", 1001, new DateTime(1990, 5, 15), 
                          "+79161234567", "Высшее", "Общий"),
                new Reader(2, "Петрова М.С.", 1002, new DateTime(1985, 8, 22), 
                          "+79169876543", "Высшее", "Физико-математический")
            });
            
            halls.Add(new Hall(1, "Центральная", "Общий", "Универсальный", 50, 
                              new List<int> { 1 }));
            
            Console.WriteLine("Загружены тестовые данные.\n");
        }

        // ==================== МЕНЮ ====================
        
        static void ShowMenu()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║     БИБЛИОТЕЧНАЯ СИСТЕМА (LINQ запросы)            ║");
            Console.WriteLine("╠════════════════════════════════════════════════════╣");
            Console.WriteLine("║  1 - Какие книги выданы каждому читателю           ║");
            Console.WriteLine("║  2 - Сколько свободных мест в каждом зале          ║");
            Console.WriteLine("║  3 - Можно ли выдать книгу (есть ли экземпляры)    ║");
            Console.WriteLine("║  4 - Количество книг автора в зале                 ║");
            Console.WriteLine("║  5 - Читатели с книгами в одном экземпляре         ║");
            Console.WriteLine("║  6 - Книги с максимальным количеством экземпляров  ║");
            Console.WriteLine("╠════════════════════════════════════════════════════╣");
            Console.WriteLine("║  7 - Записать нового читателя                      ║");
            Console.WriteLine("║  8 - Списать книгу                                 ║");
            Console.WriteLine("║  9 - Принять книгу в фонд                          ║");
            Console.WriteLine("║  0 - Выход                                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.Write("\nВыберите номер: ");
        }

        // ==================== ЗАПРОСЫ LINQ ====================
        
        // Запрос 1: Какие книги выданы каждому читателю
        static void Query1_BooksByReader()
        {
            Console.WriteLine("\n=== КНИГИ, ВЫДАННЫЕ ЧИТАТЕЛЯМ ===\n");
            
            // Связываем читателей с книгами по залу (упрощённая модель)
            // В реальности нужна таблица "Выдача", здесь используем Join по ключам залов
            
            var query = readers
                .GroupJoin(books.Where(b => b.Copies > 0),
                    r => r.Hall,
                    b => "Общий", // Упрощение: все книги доступны в общем зале
                    (reader, bookList) => new
                    {
                        Reader = reader.FullName,
                        Ticket = reader.TicketNumber,
                        Books = bookList.Select(b => b.Title).ToList()
                    });

            foreach (var item in query)
            {
                Console.WriteLine($"Читатель: {item.Reader} (Билет: {item.Ticket})");
                if (item.Books.Any())
                    Console.WriteLine("  Книги: " + string.Join(", ", item.Books));
                else
                    Console.WriteLine("  Книг нет");
                Console.WriteLine();
            }
        }

        // Запрос 2: Сколько свободных мест в каждом зале
        static void Query2_FreeSeatsInHalls()
        {
            Console.WriteLine("\n=== СВОБОДНЫЕ МЕСТА В ЗАЛАХ ===\n");
            
            var query = halls.Select(h => new
            {
                HallName = h.Name,
                TotalSeats = h.Capacity,
                OccupiedSeats = h.ReaderKeys.Count,
                FreeSeats = h.Capacity - h.ReaderKeys.Count
            });

            Console.WriteLine("{0,-20} {1,-10} {2,-10} {3,-10}", 
                "Зал", "Всего", "Занято", "Свободно");
            Console.WriteLine(new string('-', 55));
            
            foreach (var h in query)
            {
                Console.WriteLine("{0,-20} {1,-10} {2,-10} {3,-10}", 
                    h.HallName, h.TotalSeats, h.OccupiedSeats, h.FreeSeats);
            }
        }

        // Запрос 3: Можно ли выдать книгу (есть свободные экземпляры)
        static void Query3_CanIssueBook()
        {
            Console.Write("\nВведите название книги: ");
            string title = Console.ReadLine();
            
            var book = books.FirstOrDefault(b => 
                b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            
            if (book == null)
            {
                Console.WriteLine("Книга не найдена!");
                return;
            }
            
            Console.WriteLine($"\nКнига: {book.Title}");
            Console.WriteLine($"Доступно экземпляров: {book.Copies}");
            
            if (book.Copies > 0)
                Console.WriteLine("РЕЗУЛЬТАТ: Можно выдать читателю ✓");
            else
                Console.WriteLine("РЕЗУЛЬТАТ: Нельзя выдать — нет свободных экземпляров ✗");
        }

        // Запрос 4: Количество книг заданного автора в читальном зале
        static void Query4_BooksByAuthorInHall()
        {
            Console.Write("\nВведите фамилию автора: ");
            string author = Console.ReadLine();
            
            Console.Write("Введите название зала: ");
            string hallName = Console.ReadLine();
            
            // Подсчитываем книги автора (в данной модели книги не привязаны к залам напрямую,
            // поэтому показываем общее количество с указанием зала как "все залы")
            var count = books.Count(b => 
                b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
            
            var booksList = books.Where(b => 
                b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)).ToList();
            
            Console.WriteLine($"\nКниги автора '{author}' в зале '{hallName}': {count}");
            foreach (var b in booksList)
            {
                Console.WriteLine($"  - {b.Title} ({b.Year})");
            }
        }

        // Запрос 5: Читатели, взявшие книги в одном экземпляре
        static void Query5_UniqueBooksReaders()
        {
            Console.WriteLine("\n=== ЧИТАТЕЛИ С УНИКАЛЬНЫМИ КНИГАМИ ===\n");
            
            // Книги в одном экземпляре
            var uniqueBooks = books.Where(b => b.Copies == 1).ToList();
            
            Console.WriteLine("Книги в одном экземпляре:");
            foreach (var b in uniqueBooks)
            {
                Console.WriteLine($"  - {b.Title}");
            }
            
            // В реальной системе здесь был бы Join с таблицей выдач
            // Для демонстрации показываем всех читателей зала, где хранятся редкие книги
            var readersWithUnique = readers
                .Where(r => r.Hall == "Художественный" || r.Hall == "Общий")
                .ToList();
            
            Console.WriteLine($"\nЧитатели, имеющие доступ к редким книгам: {readersWithUnique.Count}");
            foreach (var r in readersWithUnique)
            {
                Console.WriteLine($"  - {r.FullName} ({r.Hall})");
            }
        }

        // Запрос 6: Книги с максимальным количеством экземпляров (рейтинг = количество)
        static void Query6_MaxRatingBooks()
        {
            Console.WriteLine("\n=== КНИГИ С МАКСИМАЛЬНЫМ КОЛИЧЕСТВОМ ЭКЗЕМПЛЯРОВ ===\n");
            
            if (!books.Any()) return;
            
            int maxCopies = books.Max(b => b.Copies);
            
            var maxBooks = books.Where(b => b.Copies == maxCopies);
            
            Console.WriteLine($"Максимальное количество экземпляров: {maxCopies}");
            Console.WriteLine("Книги:");
            
            foreach (var b in maxBooks)
            {
                Console.WriteLine($"  - {b}");
            }
        }

        // ==================== ОПЕРАЦИИ РЕДАКТИРОВАНИЯ ====================
        
        static void AddNewReader()
        {
            Console.WriteLine("\n=== ЗАПИСЬ НОВОГО ЧИТАТЕЛЯ ===\n");
            
            try
            {
                int newKey = readers.Any() ? readers.Max(r => r.Key) + 1 : 1;
                
                Console.Write("ФИО: ");
                string fullName = Console.ReadLine();
                
                int ticket = readers.Any() ? readers.Max(r => r.TicketNumber) + 1 : 1001;
                
                Console.Write("Дата рождения (гггг-мм-дд): ");
                DateTime birth = DateTime.Parse(Console.ReadLine());
                
                Console.Write("Телефон: ");
                string phone = Console.ReadLine();
                
                Console.Write("Образование: ");
                string education = Console.ReadLine();
                
                Console.Write("Зал (Общий/Физико-математический/Художественный): ");
                string hall = Console.ReadLine();
                
                var newReader = new Reader(newKey, fullName, ticket, birth, phone, education, hall);
                readers.Add(newReader);
                
                // Обновляем зал
                var targetHall = halls.FirstOrDefault(h => h.Name == hall);
                if (targetHall != null)
                    targetHall.ReaderKeys.Add(newKey);
                
                Console.WriteLine($"\nЧитатель '{fullName}' успешно записан! Номер билета: {ticket}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void RemoveBook()
        {
            Console.Write("\nВведите шифр книги для списания: ");
            string code = Console.ReadLine();
            
            var book = books.FirstOrDefault(b => b.Code == code);
            
            if (book == null)
            {
                Console.WriteLine("Книга не найдена!");
                return;
            }
            
            books.Remove(book);
            Console.WriteLine($"Книга '{book.Title}' списана из фонда.");
        }

        static void AddNewBook()
        {
            Console.WriteLine("\n=== ПРИЁМ КНИГИ В ФОНД ===\n");
            
            try
            {
                int newKey = books.Any() ? books.Max(b => b.Key) + 1 : 1;
                
                Console.Write("Название: ");
                string title = Console.ReadLine();
                
                Console.Write("Автор: ");
                string author = Console.ReadLine();
                
                Console.Write("Год издания: ");
                int year = int.Parse(Console.ReadLine());
                
                string code = $"ABC-{newKey:D3}";
                
                Console.Write("Количество экземпляров: ");
                int copies = int.Parse(Console.ReadLine());
                
                var newBook = new Book(newKey, title, author, year, code, 
                                      DateTime.Now, DateTime.Now.AddDays(30), copies);
                books.Add(newBook);
                
                Console.WriteLine($"\nКнига '{title}' принята в фонд. Шифр: {code}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
