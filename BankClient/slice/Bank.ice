module Bank { 

  exception IncorrectData { string reason; };
  exception RequestRejected { string reason; };
  exception IncorrectAccountNumber {};
  exception IncorrectAmount {};
  exception NoSuchAccount {};

  enum currency {PLN, USD, EUR, CHF}; 

  enum accountType {SILVER, PREMIUM};

  // amount: kwota w walucie krajowej (PLN) przemno�ona przez 100 - 11,50 z� to 1150

  interface Account {
    int getBalance ();
    string getAccountNumber();
    void transfer(string accountNumber, int amount) throws IncorrectAccountNumber, IncorrectAmount;
  };


  // amount: kwota w walucie krajowej (PLN) przemno�ona przez 100 - 11,50 z� to 1150
  // curr: waluta, w kt�rej ma by� wzi�ty kredyt, w��czaj�c PLN
  // period: czas w miesi�cach
  // totalCost: ca�kowity koszt kredytu w walucie narodowej w chwili bie��cej
  // interestRate: aktualne ��czne oprocentowanie
  // zwracane wielko�ci powinny bra� pod uwag� uzyskiwane z notyfikacji kursy waluty obcej i wysoko�� jej oprocentowania na rynku bankowym, prowizj� banku oraz ewentualne zni�ki dla wybranych klient�w

  interface PremiumAccount extends Account {
    void calculateLoan(int amount, currency curr, int period, out int totalCost, out float interestRate) throws IncorrectData;
  };


struct PersonalData
{
  string firstName;
  string lastName;
  string NationalIDNumber; //PESEL
};


  //accountID: powinno by� u�ywane jako name w Identity identyfikuj�cym konto.
  //System musi wiedzie�, kt�re konta aktualnie (od za�o�enia do usuni�cia) istniej�.

  interface BankManager
  {
    void createAccount(PersonalData data, accountType type, out string accountID) throws IncorrectData, RequestRejected;
    void removeAccount(string accountID) throws IncorrectData, NoSuchAccount;
  };
};

