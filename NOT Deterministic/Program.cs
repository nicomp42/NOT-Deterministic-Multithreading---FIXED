/*
 * Bill Nicholson
 * FIXED FIXED FIXED
 * nicholdw@ucmail.uc.edu
 * Threading example that is NOT deterministic
 * Run the app several times and observe that the balance changes.
 * The code is not atomic. It will not execute deterministically
 * FIXED FIXED FIXED
 */

using System;
using System.Threading;

public class BankAccount {
    private double balance; 
    public BankAccount(double balance){this.balance = balance;}
    public void Deposit(double amount) {
        lock (this) {   // FIXED by adding a lock in Deposit() and Withdraw() methods
            double tmp = balance;
            Thread.Sleep((new Random()).Next(1000));    // Add in some randomness for the real world
            tmp = tmp + amount;
            balance = tmp;
        }
    }
    public void Withdraw(double amount) {
        lock (this) {  // FIXED by adding a lock in Deposit() and Withdraw() methods
            double tmp = balance;
            Thread.Sleep((new Random()).Next(1000));    // Add in some randomness for the real world
            tmp = tmp - amount;
            balance = tmp;
        }
    }
    public double getBalance() {return balance;}
}

public class Deposit
{
    BankAccount bankAccount;
    double amount;
    public Deposit(BankAccount bankAccount, double amount) {this.bankAccount = bankAccount; this.amount = amount;}
    // This method that will be called when the thread is started, then the thread will end when the methods end
    public void MakeDeposit()
    {
        bankAccount.Deposit(amount);
    }
};

public class Withdraw
{
    BankAccount bankAccount;
    double amount;
    public Withdraw(BankAccount bankAccount, double amount) {this.bankAccount = bankAccount; this.amount = amount;}
    // This method that will be called when the thread is started, then the thread will end when the methos ends
    public void MakeWithdrawal()
    {
        bankAccount.Withdraw(amount);
    }
};

public class Simple
{
    // Entry Point
    public static int Main()
    {
        Console.WriteLine("NON Deterministic thread example");

        // Some poor schlep has $500 in their account. Not for long...
        BankAccount bankAccount = new BankAccount(500.0);

        // Create the deposit and withdraw threads. These represent two physical ATMs or banks, etc.
        Deposit deposit = new Deposit(bankAccount, 10);
        Withdraw withdraw = new Withdraw(bankAccount, 100);

        // Create the thread object, passing in the desired method via a ThreadStart delegate. This does not start the thread.
        Thread depositThread = new Thread(new ThreadStart(deposit.MakeDeposit));
        Thread withdrawThread = new Thread(new ThreadStart(withdraw.MakeWithdrawal));

        // Start the threads. Two people at two ATMs
        depositThread.Start(); // Deposit $10
        withdrawThread.Start(); // Withdraw $100
        // Eventually there should be 500 + 10 - 100 = 410 in the account

        // Wait until both Threads finish. This just to keep this method from finishing first, which would kill the other threads. 
        depositThread.Join();
        withdrawThread.Join();

        Console.WriteLine();
        Console.WriteLine("main() has finished");
        Console.WriteLine("The account balance is now " + bankAccount.getBalance());

        return 0;
    }
}
