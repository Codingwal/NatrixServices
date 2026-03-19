namespace NatrixServices;

public record DnsBlockerConfig(bool Block, List<string> IPAddresses, List<string> DomainsToBlock);