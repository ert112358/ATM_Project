import { Tabs } from "expo-router";
import React from "react";

import { HapticTab } from "@/components/haptic-tab";
import MaterialIcons from "@expo/vector-icons/MaterialIcons";
import { Colors } from "@/constants/theme";
import { useColorScheme } from "@/hooks/use-color-scheme";
import MaterialCommunityIcons from "@expo/vector-icons/MaterialCommunityIcons";

export default function TabLayout() {
  const colorScheme = useColorScheme();

  return (
    <Tabs
      screenOptions={{
        tabBarActiveTintColor: Colors[colorScheme ?? "light"].tint,
        headerShown: false,
        tabBarButton: HapticTab,
      }}
    >
      <Tabs.Screen
        name="balance"
        options={{
          title: "Balance",
          tabBarIcon: ({ color }) => (
            <MaterialIcons size={28} name="balance" color={color} />
          ),
        }}
      />
      <Tabs.Screen
        name="deposit"
        options={{
          title: "Deposit",
          tabBarIcon: ({ color }) => (
            <MaterialCommunityIcons name="cash-plus" size={24} color={color} />
          ),
        }}
      />
      <Tabs.Screen
        name="withdraw"
        options={{
          title: "Withdraw",
          tabBarIcon: ({ color }) => (
            <MaterialCommunityIcons
              name="cash-refund"
              size={24}
              color={color}
            />
          ),
        }}
      />
      <Tabs.Screen
        name="changepassword"
        options={{
          title: "Change Password",
          tabBarIcon: ({ color }) => (
            <MaterialIcons name="key" size={24} color={color} />
          ),
        }}
      />
    </Tabs>
  );
}
