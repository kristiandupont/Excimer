using System.Text;
using System.Net;

namespace Excimer
{
    public class HtmlRenderer
    {
        private StringBuilder _stringBuilder = new StringBuilder();

        private void Append(params string[] strings)
        {
            foreach (var s in strings)
                _stringBuilder.Append(s);
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }

        private string targetClass;
        public void BeginSettingsForm(string targetClass)
        {
            this.targetClass = targetClass;
            Append("<form id='", targetClass, "_form'>");
            //Append("<input type='hidden' name='target' value='", targetClass, "' />");
        }

        public void AddSegment(string title)
        {
            Append("<h1>", title, "</h1>");
        }

        public void AddMappedCheckbox(string title, string name, bool initialValue)
        {
            Append("<div class='controls'>");
            Append("<label class='checkbox'>");
            Append("<input type='checkbox' class='mappedCheckBox' name='", name, "' value='True' ");
            if (initialValue) Append("checked='checked' ");
            Append("/>");
            Append(title);
            Append("</label>");
            Append("</div>");
        }

        public void AddMappedTextArea(string name, string initialValue)
        {
            Append("<div class='controls'>");
            Append("<textarea class='input-xlarge' style='margin-left: 18px' name='", name, "'>", WebUtility.HtmlEncode(initialValue), "</textarea>");
            Append("</div>");
        }

		public void AddMappedTextInput(string title, string name, string initialValue)
		{
			Append("<div class='controls'>");
			Append("<label class='control-label' for='", name, "'>", title, "</label>");
			Append(string.Format("<input style=\"margin-left: 18px\" type=\"text\" name=\"{0}\" value=\"{1}\"/>", name, WebUtility.HtmlEncode(initialValue)));
			Append("</div>");
		}

        public void EndSettingsForm()
        {
            Append("</form>");
            Append("<script>");
            Append("$('#", targetClass, "_form .mappedCheckBox').click(function() { updateConfigVar('", targetClass, "', $(this).attr('name'), ($(this).attr('checked') == 'checked') ? 'True' : 'False');  });");
            Append("$('#", targetClass, "_form textarea').keydown(function() { updateConfigVar('", targetClass, "', $(this).attr('name'), $(this).val());  });");
			Append("$('#", targetClass, "_form input').keydown(function() { updateConfigVar('", targetClass, "', $(this).attr('name'), $(this).val());  });");

            Append("</script>");

            //Append("<input id='", targetClass, "_save' type='submit' value='Save'></input>");
            //Append("</form>");
            //Append("<script>");
            //Append("$('#" + targetClass + "_form').submit(function() {");
            //Append("$('#" + targetClass + "_save').attr('value', 'Saving');");
            //Append("var post_data = $('#" + targetClass + "_form').serialize();");
            //Append("console.log('Serialized data: ', post_data);");
            //Append("$.post('api/UpdateConfig', post_data, function(data) { $('#" + targetClass + "_save').attr('value', 'Saved'); $('#" + targetClass + "_form *').change(function() { $('#" + targetClass + "_save').attr('value', 'Save'); }); });");
            //Append("return false;");
            //Append("});");
            //Append("</script>");
        }
    }
}
