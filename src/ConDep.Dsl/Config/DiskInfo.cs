using System;

namespace ConDep.Dsl.Config
{
  [Serializable]
  public class DiskInfo
  {
    public string DeviceId { get; set; }
    public long SizeInKb { get; set; }
    public long FreeSpaceInKb { get; set; }
    public string Name { get; set; }
    public string FileSystem { get; set; }
    public string VolumeName { get; set; }

    public long SizeInMb => SizeInKb / 1024;
    public int SizeInGb => Convert.ToInt32(SizeInMb) / 1024;
    public int SizeInTb => Convert.ToInt32(SizeInGb) / 1024;

    public long FreeSpaceInMb => FreeSpaceInKb / 1024;
    public int FreeSpaceInGb => Convert.ToInt32(FreeSpaceInMb) / 1024;
    public int FreeSpaceInTb => Convert.ToInt32(FreeSpaceInGb) / 1024;

    public long UsedInKb => SizeInKb - FreeSpaceInKb;
    public long UsedInMb => UsedInKb / 1024;
    public int UsedInGb => Convert.ToInt32(UsedInMb) / 1024;
    public int UsedInTb => Convert.ToInt32(UsedInGb) / 1024;

    public int PercentUsed => SizeInKb == 0 ? 0 : Convert.ToInt32(UsedInKb * 100 / SizeInKb);
    public int PercentFree => SizeInKb == 0 ? 0 : 100 - PercentUsed;
  }
}