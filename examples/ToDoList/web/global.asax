<%@ import Namespace="Bamboo.Prevalence" %>
<%@ import Namespace="Bamboo.Prevalence.Configuration" %>
<script language="C#" runat="server">
void Application_OnEnd()
{
	PrevalenceEngine engine = PrevalenceSettings.Current.GetEngine("todo");
	engine.TakeSnapshot();
}
</script>
