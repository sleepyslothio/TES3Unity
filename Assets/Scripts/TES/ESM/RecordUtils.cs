namespace TESUnity.ESM
{
	public static class RecordUtils
	{
		public static string GetModelFileName(Record record)
		{
			if (record is STATRecord)
			{
				return ((STATRecord)record).MODL.value;
			}
			else if (record is DOORRecord)
			{
				return ((DOORRecord)record).MODL.value;
			}
			else if (record is MISCRecord)
			{
				return ((MISCRecord)record).MODL.value;
			}
			else if (record is WEAPRecord)
			{
				return ((WEAPRecord)record).MODL.value;
			}
			else if (record is CONTRecord)
			{
				return ((CONTRecord)record).MODL.value;
			}
			else if (record is LIGHRecord)
			{
				return ((LIGHRecord)record).MODL.value;
			}
			else if (record is ARMORecord)
			{
				return ((ARMORecord)record).MODL.value;
			}
			else if (record is CLOTRecord)
			{
				return ((CLOTRecord)record).MODL.value;
			}
			else if (record is REPARecord)
			{
				return ((REPARecord)record).MODL.value;
			}
			else if (record is ACTIRecord)
			{
				return ((ACTIRecord)record).MODL.value;
			}
			else if (record is APPARecord)
			{
				return ((APPARecord)record).MODL.value;
			}
			else if (record is LOCKRecord)
			{
				return ((LOCKRecord)record).MODL.value;
			}
			else if (record is PROBRecord)
			{
				return ((PROBRecord)record).MODL.value;
			}
			else if (record is INGRRecord)
			{
				return ((INGRRecord)record).MODL.value;
			}
			else if (record is BOOKRecord)
			{
				return ((BOOKRecord)record).MODL.value;
			}
			else if (record is ALCHRecord)
			{
				return ((ALCHRecord)record).MODL.value;
			}
			else if (record is CREARecord)
			{
				return ((CREARecord)record).MODL.value;
			}
			else if (record is NPC_Record)
			{
				var npc = (NPC_Record)record;
				return npc.MODL != null ? npc.MODL.value : null;
			}
			else
			{
				return null;
			}
		}
	}
}
