using EdgeGateway.Byte;

namespace EdgeGateway.Channel;

public interface IByteChannel : IChannel, IByteReader, IByteWriter;