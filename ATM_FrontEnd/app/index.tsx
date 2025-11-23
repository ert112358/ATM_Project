import { Redirect } from "expo-router";

export default function Index() {
  // Redirect to your tab layout
  return <Redirect href="/(auth)/login" />;
}
