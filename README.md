
Zadanie zostało zrealizowane jako aplikacja w architekturze warstwowej z podziałem na warstwy:
• **Data** - warstwa dostępu do danych zawierająca bazę danych oraz repozytoria obsługujące żądania do niej

• **Application** - warstwa zawierająca serwisy z logiką biznesową

• **Core** - warstwa domenowa zawierająca encje, DTO oraz interfejsy repozytoriów i serwisów.

• **API** - warstwa uruchomieniowa aplikacji pliki konfiguracyjne czy rejestrację zależności. W tym przypadku nie rozbudowywałem jej o kontrolery do komunikacji z klientem czy middleware błędów, ponieważ nie było to konieczne do wykonania zadania.


# Zadanie 1 
W tym zadaniu zdecydowałem się przechowywać **EmployeesStructure** tylko w pamięci serwisu. W prawdziwym systemie w celu dalszej optymalizacji zapisywałbym tę strukturę w bazie, lecz w zadaniu nie było to wymagane.
Metody **FillEmployeesStructure** oraz **GetSupieriorRowOfEmployee** zawierające implementacje znajdują się w klasie EmployeeService w warstwie **Application**. W implementacji zdecydowałem się na uniknięcie rekurencji w tym celu 
skorzystałem z iteracyjnego podejścia z użyciem pętli po pracownikach oraz HashSetu do wykrywania cykli w przypadku błędnie podanych danych. Metody przetestowałem za pomocą testów jednostkowych z danymi z przykładu.

# Zadanie 2
Zapytania LINQ EF napisałem w EmployeeRepository znajdującym się w warstwie **Data**. W przypadku podpunktu a) zdecydowałem się na napisanie metody ogólnej nieograniczającej do konkretnego zespołu czy roku.
Natomiast wywołania na wymaganych danych przetestowałem w testach jednostkowych. W podpunkcie b) z powodu treści ( za dni zużyte uznajemy wszystkie dni we wnioskach urlopowych, które są w całości datą
przeszłą) zinterpretowałem, że nie pobieram wniosków, które nie zakończyły się przed dniem pobrania zapytania. Dodatkowo w kwestii urlopów rozpoczętych w roku poprzednim a zakończonych w bieżącym roku,
zdecydowałem się te wnioski również pobierać mając świadomość że następnie w serwisie trzeba by było odliczać dni z poprzedniego roku. Alternatywnie w tym wypadku mógłbym zastosować funkcję EF. Functions.DateDiffDay 
co jednak byłoby problematyczne, ponieważ nie wszystkie bazy obsługują tę funkcje a także utrudniało by to możliwość testowania In-Memory.

# Zadanie 3 
Metoda **CountFreeDaysForEmployee** znajduje się w **EmployeeService** w warstwie **Application** W zadaniu z powodu treści, aby uzupełnić metodę, która zwraca typ int,
zdecydowałem się uwzględniać tylko pełne dni, wnioskując, że **isPartialVacation** i **numberOfHours** we wniosku urlopowym służy do składania wniosków na niepełne dni,
założyłem w dodatku, że **isPartialVacation** które jest w typie int służyć miało raczej jako tryb bool. Dodatkowo, jako że w tym zadaniu operujemy w samym LINQ bez 
EF to zastosowałem agregację Sum() z logiką obliczeniową węwnątrz lambdy, dzięki czemu w przypadku urlopów na przekraczających nowy rok ucina od razu dni nieprzypadające do wymaganego roku.

# Zadanie 4
Metoda **IfEmployeeCanRequestVacation** znajduje się w **EmployeeService** w warstwie **Application**. W zadaniu sprawdzam dni wolne danego pracownika na podstawie metod napisanej w poprzednim zadaniu.
Dodałem możliwość podania w metodzie daty startowej i końcowej urlopu, w przypadku których podania sprawdzane jest oprócz posiadania wystarczającej ilości dni, również czy urlop nie nachodzi na inny zgłoszony wcześniej wniosek urlopowy.

# Zadanie 5
Wykonałem testy podstawowe zarówno dla metody z zadania 4, jak i dla wcześniejszych. Do testowania metod wykorzystałem framework NUnit. Dla zachowania izolacji danych każdy test tworzy nowy kontekst bazy danych w pamięc, dzięki czemu testy są niezależne i powtarzalne.

# Zadanie 6
**Eager Loading (Include/ ThenInclude)** - umożliwia w zapytaniu wczytywanie powiązanych danych, co pozwala uniknąć tzw. N+1 problem. Gdzie, zamiast wykonywać osobne zapytania dla każdej tabeli, pobieramy wszystkie dane jednocześnie.

**Select Projection** - ograniczenie pobranych kolumn tylko do realnie potrzebnych. Zastosowałem to w zadaniu 2 w podpunkcie b) gdzie, zamiast pobierać całego Usera z doczytaniem tabeli Vacation stworzyłem EmployeeVacationUsageDto, do którego pobierałem tylko niezbędne informacje z tych dwóch tabel.

Dodatkowo w przypadku danych statycznych jak VacationPackage z zadania 3, gdzie dane się praktycznie nie zmieniaja lub są wspólne dla wielu pracowników, można wykorzystać cache aplikacyjny, aby całkowicie uniknąć ponownych zapytań tej tabeli.

Kolejnym sposobem niezwiązanym bezpośrednio z optymalizacją SQL, ale z optymalizacją działania EF CORE jest zastosowanie AsNoTracking() dla zapytań, które jak w przypadku danych do zadania 3 pobierają tylko dane i niczego nie modyfikują w związku z czym zbędne jest śledzenie stanu encji które wyłącza ta funkcja.

