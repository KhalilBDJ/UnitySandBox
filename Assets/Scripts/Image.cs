using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Image
{
   public byte[] image;
   public string sessionId;

   public Image(byte[] image, string sessionId)
   {
      this.image = image;
      this.sessionId = sessionId;
   }
}
