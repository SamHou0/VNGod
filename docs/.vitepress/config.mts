import { defineConfig } from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  lang: 'zh-CN',
  titleTemplate: 'VNGod',
  title: "VNGod",
  description: "A visual novel manager, but simple and effective.",
  sitemap: {
    hostname: 'https://vngod.samhou.moe'
  },
  lastUpdated: true,
  cleanUrls: true,
  markdown: {
    lineNumbers: true
  },
  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [
      { text: '主页', link: '/' },
      { text: '文档', link: '/description' },
      { text: '下载', link: '/download' }
    ],

    sidebar: [
      {
        text: '从这里开始',
        items: [
          { text: '简介', link: '/description' },
          { text: '快速开始', link: '/start' }
        ]
      }
    ],
    socialLinks: [
      { icon: 'github', link: 'https://github.com/SamHou0/VNGod' },
      { icon: 'bilibili', link: 'https://space.bilibili.com/456964528' },
      { icon: 'rss', link: 'https://blog.samhou.top' }
    ]
  }
})