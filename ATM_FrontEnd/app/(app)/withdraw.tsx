import React, { useState } from "react";
import { useFocusEffect } from "@react-navigation/native";
import * as SecureStore from "expo-secure-store";
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  TouchableOpacity,
  ActivityIndicator,
  Alert,
} from "react-native";

export default function WithdrawScreen() {
  const token = SecureStore.getItem("token");

  const [balance, setBalance] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [amount, setAmount] = useState("");

  // STEP 1 — load balance (no amount)
  async function fetchBalance() {
    try {
      setLoading(true);

      const res = await fetch(
        `http://zeropage.it:5001/api/viewbalance?token=${token}`,
      );
      const json = await res.json();

      setBalance(json.balance);
    } catch (err) {
      console.error(err);
      Alert.alert("Error", "Unable to load balance.");
    } finally {
      setLoading(false);
    }
  }

  // STEP 2 — do the withdraw (with amount)
  async function handleWithdraw() {
    if (!amount || isNaN(Number(amount))) {
      Alert.alert("Error", "Please enter a valid number.");
      return;
    }

    const value = Number(amount);

    if (value <= 0) {
      Alert.alert("Error", "Amount must be positive.");
      return;
    }

    try {
      setLoading(true);

      const res = await fetch(
        `http://zeropage.it:5001/api/withdraw?token=${token}&amount=${value}`,
      );

      if (!res.ok) {
        Alert.alert("Error", "Withdraw failed.");
        return;
      }

      const newBalance = await res.json();
      setBalance(newBalance.balance ?? newBalance);
      setAmount("");

      Alert.alert("Success", `You withdrew ${value}€`);
    } catch (err) {
      console.error(err);
      Alert.alert("Error", "Failed to contact server.");
    } finally {
      setLoading(false);
    }
  }

  useFocusEffect(
    React.useCallback(() => {
      fetchBalance();
    }, []),
  );

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Current Balance</Text>

      {loading && balance === null ? (
        <ActivityIndicator size="large" />
      ) : (
        <Text style={styles.balanceValue}>
          {balance !== null ? `${balance} €` : "—"}
        </Text>
      )}

      <Text style={styles.subtitle}>Withdraw Amount</Text>

      <TextInput
        style={styles.input}
        keyboardType="numeric"
        value={amount}
        onChangeText={setAmount}
        placeholder="Enter amount"
      />

      <TouchableOpacity
        style={styles.button}
        onPress={handleWithdraw}
        disabled={loading}
      >
        <Text style={styles.buttonText}>Withdraw</Text>
      </TouchableOpacity>
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
  },

  balanceValue: {
    fontSize: 40,
    fontWeight: "bold",
    marginVertical: 10,
  },

  subtitle: {
    marginTop: 20,
    fontSize: 20,
    fontWeight: "600",
    marginBottom: 10,
  },

  input: {
    borderWidth: 1,
    borderColor: "#cccccc",
    borderRadius: 10,
    padding: 12,
    fontSize: 20,
    marginBottom: 20,
  },

  button: {
    backgroundColor: "#1e40af",
    padding: 14,
    borderRadius: 12,
    alignItems: "center",
  },

  buttonText: {
    color: "white",
    fontSize: 20,
    fontWeight: "600",
  },
});
