using System;
using System.IO;
using Newtonsoft.Json;
using desktop_client.Model;
namespace desktop_client.Data
{
    public class DraftStore
    {
        public int OwnerUserId { get; private set; }
        public int OwnerPartnerId { get; private set; }
        private readonly string file;
        public DraftStore(int userId, int partnerId)
        {
            OwnerUserId = userId; OwnerPartnerId = partnerId;
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NwsdbDesktop");
            Directory.CreateDirectory(folder);
            file = Path.Combine(folder, userId + "-" + partnerId + ".json");
        }
        public PaymentDraft Load()
        {
            if (!File.Exists(file)) return null;
            var draft = JsonConvert.DeserializeObject<PaymentDraft>(File.ReadAllText(file));
            if (draft == null || draft.OwnerUserId != OwnerUserId || draft.OwnerPartnerId != OwnerPartnerId)
                throw new InvalidDataException("The saved draft is not valid for this user.");
            return draft;
        }
        public void Save(PaymentDraft draft)
        {
            if (draft.OwnerUserId != OwnerUserId || draft.OwnerPartnerId != OwnerPartnerId) throw new InvalidOperationException("Draft owner mismatch.");
            var temporary = file + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(JsonConvert.SerializeObject(draft)); writer.Flush(); stream.Flush(true);
                }
                File.Move(temporary, file);
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
        public void Clear() { if (File.Exists(file)) File.Delete(file); }
    }
}
