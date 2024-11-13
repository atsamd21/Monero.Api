namespace Monero.Models;

public class MoneroSettings
{
    /// <summary>
    /// Address of monerod
    /// </summary>
    public string DaemonAddress { get; set; } = string.Empty;
    /// <summary>
    /// Parsed separately for daemon and wallet rpc but a network string maps to a specific port as per Monero's standard ports.
    /// </summary>
    public string Network { get; set; } = string.Empty;
    /// <summary>
    /// This is the directory of the monero blockchain.
    /// </summary>
    public string DataDirectory { get; set; } = string.Empty;
    /// <summary>
    /// Confirmations needed to mark payment as paid.
    /// </summary>
    public int Confirmations { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool UseRemoteNode { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string APIKey { get; set; } = string.Empty;
}
