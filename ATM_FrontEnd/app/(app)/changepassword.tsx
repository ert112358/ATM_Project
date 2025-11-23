import React, { useState, useEffect } from "react";
import { View, Text, TextInput, StyleSheet, Button, Alert } from "react-native";
import * as SecureStore from "expo-secure-store";
import { useRouter } from "expo-router";

export default function ChangePasswordScreen() {
  const router = useRouter();
  const token = SecureStore.getItem("token");

  const [username, setUsername] = useState<string | null>(null);
  const [oldPwd, setOldPwd] = useState("");
  const [newPwd, setNewPwd] = useState("");
  const [newPwd2, setNewPwd2] = useState("");

  const passwordRegex =
    /^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[^\w\d\s:])([^\s]){8,16}$/;

  async function fetchUsername() {
    try {
      const res = await fetch(
        `http://zeropage.it:5001/api/viewbalance?token=${token}`,
      );
      const json = await res.json();
      setUsername(json.name);
    } catch (err) {
      console.error(err);
    }
  }

  useEffect(() => {
    fetchUsername();
  }, []);

  async function handleChangePassword() {
    if (!username) {
      Alert.alert("Error", "Could not fetch username.");
      return;
    }

    if (!oldPwd || !newPwd || !newPwd2) {
      Alert.alert("Error", "Please fill in all fields.");
      return;
    }

    if (newPwd !== newPwd2) {
      Alert.alert("Error", "New passwords do not match.");
      return;
    }

    if (!passwordRegex.test(newPwd)) {
      Alert.alert(
        "Invalid Password",
        "New password must be 8–16 characters and include:\n• A digit\n• An uppercase letter\n• A lowercase letter\n• A special character\n• No spaces",
      );
      return;
    }

    try {
      const res = await fetch(
        `http://zeropage.it:5001/api/changepassword?username=${username}&oldpassword=${encodeURIComponent(
          oldPwd,
        )}&newpassword=${encodeURIComponent(newPwd)}`,
      );

      if (res.status === 200) {
        Alert.alert("Success", "Password changed successfully!");

        // Clear old token
        await SecureStore.deleteItemAsync("token");

        // Redirect to login screen
        router.replace("/login");

        return;
      } else if (res.status === 401) {
        Alert.alert("Unauthorized", "Old password is incorrect.");
      } else {
        Alert.alert("Error", "Something went wrong.");
      }
    } catch (err) {
      console.error(err);
      Alert.alert("Error", "Network or server issue.");
    }
  }

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Change Password</Text>

      <TextInput
        placeholder="Old password"
        secureTextEntry
        style={styles.input}
        value={oldPwd}
        onChangeText={setOldPwd}
      />

      <TextInput
        placeholder="New password"
        secureTextEntry
        style={styles.input}
        value={newPwd}
        onChangeText={setNewPwd}
      />

      <TextInput
        placeholder="Repeat new password"
        secureTextEntry
        style={styles.input}
        value={newPwd2}
        onChangeText={setNewPwd2}
      />

      <Button title="Change Password" onPress={handleChangePassword} />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 20,
  },

  title: {
    fontSize: 26,
    fontWeight: "bold",
    marginBottom: 20,
  },

  input: {
    width: "100%",
    height: 50,
    borderColor: "#ccc",
    borderWidth: 1,
    borderRadius: 10,
    marginBottom: 15,
    paddingHorizontal: 12,
    fontSize: 18,
    backgroundColor: "#fff",
  },
});
