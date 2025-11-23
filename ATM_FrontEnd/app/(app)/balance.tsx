import React, { useState } from "react";
import * as SecureStore from "expo-secure-store";
import { useFocusEffect } from "@react-navigation/native";
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  ActivityIndicator,
} from "react-native";

export default function BalanceScreen() {
  const token = SecureStore.getItem("token");
  const [balance, setBalance] = useState<number | null>(null);
  const [transactions, setTransactions] = useState([]);
  const [loading, setLoading] = useState(false);

  async function fetchBalanceAndHistory() {
    try {
      setLoading(true);

      const res = await fetch(
        `http://zeropage.it:5001/api/viewbalance?token=${token}`,
      );
      const json = await res.json();

      // json = { name, balance, transactions[] }
      setBalance(json.balance);
      setTransactions(json.transactions.reverse()); // newest first :)
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  useFocusEffect(
    React.useCallback(() => {
      fetchBalanceAndHistory();
    }, []),
  );

  function renderTransaction({ item }) {
    const isDeposit = item.type === 0;

    return (
      <View style={styles.card}>
        <Text style={styles.type}>{isDeposit ? "Deposit" : "Withdraw"}</Text>
        <Text style={[styles.amount, isDeposit ? styles.green : styles.red]}>
          {isDeposit ? "+" : "-"} {item.amount}
        </Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      {/* Current Balance */}
      <Text style={styles.title}>Current Balance</Text>

      {loading && balance === null ? (
        <ActivityIndicator size="large" />
      ) : (
        <Text style={styles.balanceValue}>{balance} €</Text>
      )}

      {/* Divider */}
      <Text style={styles.historyTitle}>Transaction History</Text>

      {loading && <ActivityIndicator size="small" />}

      <FlatList
        data={transactions}
        keyExtractor={(_, index) => index.toString()}
        renderItem={renderTransaction}
        contentContainerStyle={{ paddingBottom: 40 }}
      />
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

  historyTitle: {
    marginTop: 20,
    fontSize: 22,
    fontWeight: "600",
    marginBottom: 10,
  },

  card: {
    backgroundColor: "#ffffff",
    padding: 16,
    borderRadius: 12,
    marginBottom: 12,
    elevation: 2,
  },

  type: {
    fontSize: 18,
    fontWeight: "600",
    marginBottom: 4,
  },

  amount: {
    fontSize: 22,
    fontWeight: "bold",
  },

  green: {
    color: "#1a8b32",
  },

  red: {
    color: "#b91c1c",
  },
});
