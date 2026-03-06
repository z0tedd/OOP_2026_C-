using System;
using System.Collections.Generic;
using System.Linq;

namespace Variant2
{
    // Перечисление для периодичности
    public enum Frequency { Weekly, Monthly, Yearly }

    // Класс Person (данные автора/редактора)
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        public Person() : this("Unknown", "Unknown", DateTime.Now) { }

        public Person(string firstName, string lastName, DateTime birthDate)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}, Born: {BirthDate.ToShortDateString()}";
        }
    }

    // Класс Article (статья)
    public class Article : IComparable, IComparer<Article>
    {
        public Person Author { get; set; }
        public string Title { get; set; }
        public double Rating { get; set; }

        public Article() : this(new Person(), "Untitled", 0.0) { }

        public Article(Person author, string title, double rating)
        {
            Author = author;
            Title = title;
            Rating = rating;
        }

        public override string ToString()
        {
            return $"Title: {Title}, Author: {Author.LastName} {Author.FirstName}, Rating: {Rating}";
        }

        // IComparable - сравнение по названию статьи
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Article other = obj as Article;
            if (other != null)
                return string.Compare(this.Title, other.Title, StringComparison.Ordinal);
            throw new ArgumentException("Object is not an Article");
        }

        // IComparer<Article> - сравнение по фамилии автора
        public int Compare(Article x, Article y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return string.Compare(x.Author.LastName, y.Author.LastName, StringComparison.Ordinal);
        }
    }

    // Вспомогательный класс для сравнения по рейтингу
    public class ArticleRatingComparer : IComparer<Article>
    {
        public int Compare(Article x, Article y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.Rating.CompareTo(y.Rating);
        }
    }

    // Класс Edition (издание)
    public class Edition
    {
        protected string title;
        protected DateTime releaseDate;
        protected int circulation;

        public Edition() : this("Unknown", DateTime.Now, 0) { }

        public Edition(string title, DateTime releaseDate, int circulation)
        {
            this.title = title;
            this.releaseDate = releaseDate;
            this.Circulation = circulation;
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public DateTime ReleaseDate
        {
            get { return releaseDate; }
            set { releaseDate = value; }
        }

        public int Circulation
        {
            get { return circulation; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Circulation cannot be negative. Valid values are >= 0.");
                circulation = value;
            }
        }

        public virtual object DeepCopy()
        {
            return new Edition(title, releaseDate, circulation);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Edition other = (Edition)obj;
            return title == other.title && releaseDate == other.releaseDate && circulation == other.circulation;
        }

        public static bool operator ==(Edition e1, Edition e2)
        {
            if (ReferenceEquals(e1, null))
                return ReferenceEquals(e2, null);
            return e1.Equals(e2);
        }

        public static bool operator !=(Edition e1, Edition e2)
        {
            return !(e1 == e2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(title, releaseDate, circulation);
        }

        public override string ToString()
        {
            return $"Title: {title}, Release Date: {releaseDate.ToShortDateString()}, Circulation: {circulation}";
        }
    }

    // Класс Magazine (журнал)
    public class Magazine
    {
        private string name;
        private Frequency frequency;
        private DateTime releaseDate;
        private int circulation;
        private List<Article> articles;
        private List<Person> editors;

        public Magazine() : this("Unknown", Frequency.Monthly, DateTime.Now, 0) { }

        public Magazine(string name, Frequency frequency, DateTime releaseDate, int circulation)
        {
            this.name = name;
            this.frequency = frequency;
            this.releaseDate = releaseDate;
            this.circulation = circulation;
            this.articles = new List<Article>();
            this.editors = new List<Person>();
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Frequency Frequency
        {
            get { return frequency; }
            set { frequency = value; }
        }

        public DateTime ReleaseDate
        {
            get { return releaseDate; }
            set { releaseDate = value; }
        }

        public int Circulation
        {
            get { return circulation; }
            set { circulation = value; }
        }

        public List<Article> Articles
        {
            get { return articles; }
            set { articles = value; }
        }

        public List<Person> Editors
        {
            get { return editors; }
            set { editors = value; }
        }

        // Средний рейтинг статей
        public double AverageRating
        {
            get
            {
                if (articles == null || articles.Count == 0) return 0.0;
                return articles.Average(a => a.Rating);
            }
        }

        // Индексатор по периодичности
        public bool this[Frequency freq]
        {
            get { return frequency == freq; }
        }

        public void AddArticles(params Article[] newArticles)
        {
            if (articles == null) articles = new List<Article>();
            articles.AddRange(newArticles);
        }

        public void AddEditors(params Person[] newEditors)
        {
            if (editors == null) editors = new List<Person>();
            editors.AddRange(newEditors);
        }

        public override string ToString()
        {
            string result = $"Magazine: {name}\nFrequency: {frequency}\nRelease Date: {releaseDate.ToShortDateString()}\nCirculation: {circulation}\nAverage Rating: {AverageRating:F2}\nEditors ({editors.Count}):\n";
            foreach (var editor in editors)
                result += $"  - {editor}\n";
            result += $"Articles ({articles.Count}):\n";
            foreach (var article in articles)
                result += $"  - {article}\n";
            return result;
        }

        public string ToShortString()
        {
            return $"Magazine: {name}, Frequency: {frequency}, Release: {releaseDate.ToShortDateString()}, " +
                   $"Circulation: {circulation}, Avg Rating: {AverageRating:F2}, Editors: {editors.Count}, Articles: {articles.Count}";
        }

        // Методы сортировки
        public void SortArticlesByTitle()
        {
            articles.Sort((a, b) => string.Compare(a.Title, b.Title, StringComparison.Ordinal));
        }

        public void SortArticlesByAuthorLastName()
        {
            articles.Sort(new Article());
        }

        public void SortArticlesByRating()
        {
            articles.Sort(new ArticleRatingComparer());
        }
    }

    // Универсальный делегат
    public delegate TKey KeySelector<TKey>(Magazine mg);

    // Универсальный класс MagazineCollection<TKey>
    public class MagazineCollection<TKey>
    {
        private Dictionary<TKey, Magazine> magazines;
        private KeySelector<TKey> keySelector;

        public MagazineCollection(KeySelector<TKey> keySelector)
        {
            this.keySelector = keySelector;
            this.magazines = new Dictionary<TKey, Magazine>();
        }

        public void AddDefaults()
        {
            var mag1 = new Magazine("Science Today", Frequency.Monthly, new DateTime(2024, 1, 15), 50000);
            mag1.AddArticles(
                new Article(new Person("John", "Smith", new DateTime(1980, 5, 10)), "Quantum Physics", 4.5),
                new Article(new Person("Jane", "Doe", new DateTime(1985, 3, 20)), "AI Revolution", 4.8)
            );
            mag1.AddEditors(new Person("Bob", "Editor", new DateTime(1970, 1, 1)));

            var mag2 = new Magazine("Tech Weekly", Frequency.Weekly, new DateTime(2024, 2, 1), 30000);
            mag2.AddArticles(
                new Article(new Person("Alice", "Johnson", new DateTime(1990, 8, 15)), "New Gadgets", 3.9),
                new Article(new Person("Charlie", "Brown", new DateTime(1988, 12, 5)), "Software Trends", 4.2)
            );

            var mag3 = new Magazine("Nature Annual", Frequency.Yearly, new DateTime(2024, 6, 1), 100000);
            mag3.AddArticles(
                new Article(new Person("David", "Wilson", new DateTime(1975, 4, 10)), "Climate Change", 4.9)
            );

            AddMagazines(mag1, mag2, mag3);
        }

        public void AddMagazines(params Magazine[] newMagazines)
        {
            foreach (var mag in newMagazines)
            {
                TKey key = keySelector(mag);
                magazines[key] = mag;
            }
        }

        public override string ToString()
        {
            string result = $"Magazine Collection ({magazines.Count} items):\n\n";
            foreach (var pair in magazines)
            {
                result += $"[Key: {pair.Key}]\n{pair.Value}\n";
                result += new string('-', 50) + "\n";
            }
            return result;
        }

        public string ToShortString()
        {
            string result = $"Magazine Collection ({magazines.Count} items):\n";
            foreach (var pair in magazines)
            {
                result += $"[Key: {pair.Key}] {pair.Value.ToShortString()}\n";
            }
            return result;
        }

        // Максимальный средний рейтинг
        public double MaxAverageRating
        {
            get
            {
                if (magazines.Count == 0) return 0.0;
                return magazines.Max(m => m.Value.AverageRating);
            }
        }

        // Фильтрация по периодичности
        public IEnumerable<KeyValuePair<TKey, Magazine>> FrequencyGroup(Frequency value)
        {
            return magazines.Where(m => m.Value.Frequency == value);
        }

        // Группировка по периодичности
        public IEnumerable<IGrouping<Frequency, KeyValuePair<TKey, Magazine>>> GroupByFrequency
        {
            get
            {
                return magazines.GroupBy(m => m.Value.Frequency);
            }
        }
    }

    // Универсальный делегат для TestCollections
    public delegate KeyValuePair<TKey, TValue> GenerateElement<TKey, TValue>(int j);

    // Класс TestCollections
    public class TestCollections<TKey, TValue>
    {
        private List<TKey> keyList;
        private List<string> stringList;
        private Dictionary<TKey, TValue> keyValueDict;
        private Dictionary<string, TValue> stringValueDict;
        private GenerateElement<TKey, TValue> generateMethod;

        public TestCollections(int count, GenerateElement<TKey, TValue> generateMethod)
        {
            this.generateMethod = generateMethod;
            keyList = new List<TKey>();
            stringList = new List<string>();
            keyValueDict = new Dictionary<TKey, TValue>();
            stringValueDict = new Dictionary<string, TValue>();

            for (int i = 0; i < count; i++)
            {
                var pair = generateMethod(i);
                keyList.Add(pair.Key);
                stringList.Add(pair.Key.ToString());
                keyValueDict[pair.Key] = pair.Value;
                stringValueDict[pair.Key.ToString()] = pair.Value;
            }
        }

        public void MeasureSearchTime()
        {
            if (keyList.Count == 0) return;

            // Элементы для поиска
            TKey first = keyList[0];
            TKey middle = keyList[keyList.Count / 2];
            TKey last = keyList[keyList.Count - 1];
            TKey notFound = generateMethod(keyList.Count + 1000).Key;

            Console.WriteLine($"\n Search Time Measurements (Collection size: {keyList.Count}) \n");

            // List<TKey> Contains
            MeasureTime("List<TKey>", "First", first, () => keyList.Contains(first));
            MeasureTime("List<TKey>", "Middle", middle, () => keyList.Contains(middle));
            MeasureTime("List<TKey>", "Last", last, () => keyList.Contains(last));
            MeasureTime("List<TKey>", "Not found", notFound, () => keyList.Contains(notFound));

            // List<string> Contains
            string firstStr = first.ToString();
            string middleStr = middle.ToString();
            string lastStr = last.ToString();
            string notFoundStr = notFound.ToString();

            MeasureTime("List<string>", "First", firstStr, () => stringList.Contains(firstStr));
            MeasureTime("List<string>", "Middle", middleStr, () => stringList.Contains(middleStr));
            MeasureTime("List<string>", "Last", lastStr, () => stringList.Contains(lastStr));
            MeasureTime("List<string>", "Not found", notFoundStr, () => stringList.Contains(notFoundStr));

            // Dictionary<TKey, TValue> ContainsKey
            MeasureTime("Dictionary<TKey,TValue> (by key)", "First", first, () => keyValueDict.ContainsKey(first));
            MeasureTime("Dictionary<TKey,TValue> (by key)", "Middle", middle, () => keyValueDict.ContainsKey(middle));
            MeasureTime("Dictionary<TKey,TValue> (by key)", "Last", last, () => keyValueDict.ContainsKey(last));
            MeasureTime("Dictionary<TKey,TValue> (by key)", "Not found", notFound, () => keyValueDict.ContainsKey(notFound));

            // Dictionary<string, TValue> ContainsKey
            MeasureTime("Dictionary<string,TValue> (by key)", "First", firstStr, () => stringValueDict.ContainsKey(firstStr));
            MeasureTime("Dictionary<string,TValue> (by key)", "Middle", middleStr, () => stringValueDict.ContainsKey(middleStr));
            MeasureTime("Dictionary<string,TValue> (by key)", "Last", lastStr, () => stringValueDict.ContainsKey(lastStr));
            MeasureTime("Dictionary<string,TValue> (by key)", "Not found", notFoundStr, () => stringValueDict.ContainsKey(notFoundStr));

            // Dictionary<TKey, TValue> ContainsValue
            TValue firstValue = keyValueDict[first];
            TValue middleValue = keyValueDict[middle];
            TValue lastValue = keyValueDict[last];
            TValue notFoundValue = generateMethod(keyList.Count + 1000).Value;

            MeasureTime("Dictionary<TKey,TValue> (by value)", "First", firstValue, () => keyValueDict.ContainsValue(firstValue));
            MeasureTime("Dictionary<TKey,TValue> (by value)", "Middle", middleValue, () => keyValueDict.ContainsValue(middleValue));
            MeasureTime("Dictionary<TKey,TValue> (by value)", "Last", lastValue, () => keyValueDict.ContainsValue(lastValue));
            MeasureTime("Dictionary<TKey,TValue> (by value)", "Not found", notFoundValue, () => keyValueDict.ContainsValue(notFoundValue));
        }

        private void MeasureTime<T>(string collectionType, string elementType, T element, Func<bool> searchAction)
        {
            // Разогрев
            for (int i = 0; i < 100; i++) searchAction();

            // Измерение
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                searchAction();
            }
            sw.Stop();

            Console.WriteLine($"{collectionType,-30} | {elementType,-10} | {sw.ElapsedTicks / 10000.0:F2} ticks");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" VARIANT 2 \n");

            //  ЗАДАНИЕ 1: Сортировка статей в Magazine 
            Console.WriteLine(" TASK 1: Magazine Article Sorting \n");

            var magazine = new Magazine("Programming Monthly", Frequency.Monthly, new DateTime(2024, 3, 1), 25000);
            
            magazine.AddArticles(
                new Article(new Person("Bob", "Martin", new DateTime(1970, 1, 1)), "Clean Code", 4.7),
                new Article(new Person("Alice", "Johnson", new DateTime(1985, 5, 15)), "Algorithms", 4.2),
                new Article(new Person("Charlie", "Brown", new DateTime(1990, 8, 20)), "Design Patterns", 4.9),
                new Article(new Person("David", "Smith", new DateTime(1980, 3, 10)), "Best Practices", 3.8)
            );

            Console.WriteLine("Original Magazine:");
            Console.WriteLine(magazine);

            // Сортировка по названию статьи
            Console.WriteLine("\n Sorted by Title ");
            magazine.SortArticlesByTitle();
            Console.WriteLine(magazine);

            // Сортировка по фамилии автора
            Console.WriteLine("\n Sorted by Author Last Name ");
            magazine.SortArticlesByAuthorLastName();
            Console.WriteLine(magazine);

            // Сортировка по рейтингу
            Console.WriteLine("\n Sorted by Rating ");
            magazine.SortArticlesByRating();
            Console.WriteLine(magazine);

            //  ЗАДАНИЕ 2: MagazineCollection<string> 
            Console.WriteLine("\n TASK 2: MagazineCollection<string> \n");

            MagazineCollection<string> magCollection = new MagazineCollection<string>(m => m.Name);
            magCollection.AddDefaults();

            Console.WriteLine(magCollection.ToString());

            //  ЗАДАНИЕ 3: Операции с коллекцией 
            Console.WriteLine("\n TASK 3: Collection Operations \n");

            // Максимальный средний рейтинг
            Console.WriteLine($"Max Average Rating: {magCollection.MaxAverageRating:F2}");
            Console.WriteLine();

            // Фильтрация по периодичности
            Console.WriteLine(" Magazines with Monthly Frequency ");
            foreach (var pair in magCollection.FrequencyGroup(Frequency.Monthly))
            {
                Console.WriteLine($"[{pair.Key}] {pair.Value.ToShortString()}");
            }
            Console.WriteLine();

            // Группировка по периодичности
            Console.WriteLine(" Grouped by Frequency ");
            foreach (var group in magCollection.GroupByFrequency)
            {
                Console.WriteLine($"\nFrequency: {group.Key}");
                foreach (var pair in group)
                {
                    Console.WriteLine($"  [{pair.Key}] {pair.Value.Name}");
                }
            }

            //  ЗАДАНИЕ 4: TestCollections 
            Console.WriteLine("\n TASK 4: TestCollections<Edition, Magazine> \n");

            int count = 0;
            bool validInput = false;
            while (!validInput)
            {
                Console.Write("Enter number of elements in collections: ");
                try
                {
                    count = int.Parse(Console.ReadLine());
                    if (count > 0) validInput = true;
                    else Console.WriteLine("Please enter a positive number.");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input! Please enter an integer.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Number is too large or too small.");
                }
            }

            // Метод генерации элементов
            GenerateElement<Edition, Magazine> generator = (j) =>
            {
                var edition = new Edition($"Edition_{j}", DateTime.Now.AddDays(j), 1000 + j);
                var mag = new Magazine($"Magazine_{j}", (Frequency)(j % 3), DateTime.Now.AddDays(j), 10000 + j);
                return new KeyValuePair<Edition, Magazine>(edition, mag);
            };

            var testCollections = new TestCollections<Edition, Magazine>(count, generator);
            testCollections.MeasureSearchTime();

            Console.WriteLine("\n END OF VARIANT 2 ");
            Console.ReadKey();
        }
    }
}
